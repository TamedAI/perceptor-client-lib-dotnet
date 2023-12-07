using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels.HttpClient;

namespace Perceptor.Client.Lib.Repository.HttpClient
{
	internal class HttpClientFacade : IHttpClientFacade
	{
		private readonly System.Net.Http.HttpClient _httpClient;

		public HttpClientFacade(System.Net.Http.HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		
		public async Task<IHttpClientResponse> SendMessage(Func<HttpRequestMessage> messageFactory, CancellationToken cancellationToken)
		{
			using HttpRequestMessage message = messageFactory();
			HttpResponseMessage responseMessage = await _httpClient.SendAsync(message,
				HttpCompletionOption.ResponseContentRead, 
				cancellationToken);
			return new ClientHttpMessageResponse(responseMessage);
		}
	}
}