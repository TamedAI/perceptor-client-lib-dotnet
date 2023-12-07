using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.Configuration;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.InternalModels.HttpClient;
using Perceptor.Client.Lib.Repository.HttpClient;

namespace Perceptor.Client.Lib.Repository
{
	internal class PerceptorHttpRepository : IPerceptorRepository
	{
		private readonly IHttpClientFacade _httpClient;

		private readonly Lazy<string> _lazyAuthorizationHeader;
		private readonly Lazy<string> _lazyUrlMethodGenerate;
		private readonly Lazy<string> _lazyUrlMethodGenerateTable;
		private readonly Lazy<string> _lazyUrlMethodClassify;
		private readonly PerceptorRepositoryHttpClientSettings _settings;

		public PerceptorHttpRepository(PerceptorRepositoryHttpClientSettings settings, 
			IHttpClientFacade httpClientFacade)
		{
			_settings = settings;
			_httpClient = httpClientFacade;

			_lazyAuthorizationHeader = new Lazy<string>(() => $"Bearer {settings.ApiKey}",
				LazyThreadSafetyMode.ExecutionAndPublication);

			_lazyUrlMethodGenerate = new Lazy<string>(() => ConcatUrl(settings.Url, "generate"),
				LazyThreadSafetyMode.ExecutionAndPublication);

			_lazyUrlMethodGenerateTable = new Lazy<string>(() => ConcatUrl(settings.Url, "generate_table"),
				LazyThreadSafetyMode.ExecutionAndPublication);
			
			_lazyUrlMethodClassify = new Lazy<string>(() => ConcatUrl(settings.Url, "classify"),
				LazyThreadSafetyMode.ExecutionAndPublication);
		}

		internal static string ConcatUrl(string baseUrl, string toAdd)
		{
			return Flurl.Url.Combine(baseUrl, toAdd);
		}

		public async Task<OneOf<PerceptorSuccessResult, PerceptorError>> SendInstruction(
			PerceptorRequestPayload perceptorRequestPayload,
			CancellationToken cancellationToken)
		{
			string bodyText = BuildBody(perceptorRequestPayload);

			IHttpClientResponse result = await _httpClient.SendMessage(CreateMessageWithBody, cancellationToken);

			return await ProcessClientResponse(result);

			HttpRequestMessage CreateMessageWithBody()
			{
				return CreateHttpMessage(perceptorRequestPayload, bodyText);
			}
		}

		private HttpRequestMessage CreateHttpMessage(PerceptorRequestPayload perceptorRequestPayload, string bodyText)
		{
			var message = new HttpRequestMessage(
				HttpMethod.Post, GetUrlForMethod(perceptorRequestPayload.Method));
			message.Headers.Add("Accept", "text/event-stream");
			message.Headers.Add("Authorization", _lazyAuthorizationHeader.Value);
			message.Content = new StringContent(bodyText,
				Encoding.UTF8, "application/json");
			
			return message;
		}

		private static async Task<OneOf<PerceptorSuccessResult, PerceptorError>> ProcessClientResponse(IHttpClientResponse clientResponse)
		{
			return clientResponse switch
			{
				ClientHttpMessageResponse messageResponse => await MapHttpResponseMessage(messageResponse.Message),
				ClientErrorResponse errorResponse => errorResponse.ErrorInfo,
				_ => ErrorConstants.UnknownError
			};
		}
		private static async Task<OneOf<PerceptorSuccessResult, PerceptorError>> MapHttpResponseMessage(HttpResponseMessage response)
		{
			using HttpResponseMessage disposableMessage = response;
			
			return disposableMessage.StatusCode switch
			{
				HttpStatusCode.OK => await ParseSuccessResponse(disposableMessage),
				HttpStatusCode.Forbidden => ErrorConstants.InvalidApiKey,
				HttpStatusCode.BadRequest  => await ParseBadRequestResponse(disposableMessage),
				HttpStatusCode.NoContent =>await ParseBadRequestResponse(disposableMessage),
				_ => ErrorConstants.UnknownError
			};

			async Task<PerceptorError> ParseBadRequestResponse(HttpResponseMessage message)
			{
				string content = await message.GetResponseTextContent();
				return new PerceptorError(content.MapBadRequestContentString());
			}
		}

		private static async Task<PerceptorSuccessResult> ParseSuccessResponse(HttpResponseMessage response)
		{
			string responseContent = await response.GetResponseTextContent();
			IReadOnlyList<SsEvent> events = await SseEventParser.ParseEvents(responseContent);
			return new PerceptorSuccessResult(String.Join(String.Empty, events.Select(x => x.Data)));
		}

		private string BuildBody(PerceptorRequestPayload payload)
		{
			return RepositoryHelpers.MapToPayloadString(payload, _settings.WaitTimeout);
		}

		private string GetUrlForMethod(InstructionMethod method)
		{
			return method switch
			{
				InstructionMethod.Question => _lazyUrlMethodGenerate.Value,
				InstructionMethod.Classify => _lazyUrlMethodClassify.Value,
				InstructionMethod.Table => _lazyUrlMethodGenerateTable.Value,

				_ => throw new ArgumentException($"Method {method} not supported")
			};
		}
	}
}