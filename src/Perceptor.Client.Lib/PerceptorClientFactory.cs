using System;
using System.Net.Http;
using Perceptor.Client.Lib.Configuration;
using Perceptor.Client.Lib.Repository;
using Perceptor.Client.Lib.Repository.HttpClient;
using Perceptor.Client.Lib.Services;

namespace Perceptor.Client.Lib
{
	public static class PerceptorClientFactory
	{
		/// <summary>
		/// Create client instance using specified settings
		/// </summary>
		/// <param name="clientSettings"></param>
		/// <returns></returns>
		public static PerceptorClient CreateFromSettings(ClientSettings clientSettings)
		{
			AssertClientSettingsValid(clientSettings);
			
			var httpClient = new HttpClient()
			{
				Timeout = clientSettings.WaitTimeout
			};

			var httpClientFacade = new HttpClientFacade(httpClient);
			var pollyDecorator = new HttpClientFacadePollyDecorator(
				new HttpClientPolicySettings(clientSettings.RetryCount, clientSettings.WaitTimeout),
				httpClientFacade
			);
			var perceptorRepository = new PerceptorHttpRepository(clientSettings.ToHttpClientSettings(),
				pollyDecorator);
			var taskService = new TaskLimiterService(clientSettings.MaximalNumberOfParallelRequests);
			return new PerceptorClient(data =>
				new ContentSessionContext(taskService, perceptorRepository, data));
		}

		private static void AssertClientSettingsValid(ClientSettings clientSettings)
		{
			var validationError = clientSettings.Validate();
			if (!String.IsNullOrWhiteSpace(validationError))
			{
				throw new ArgumentException($"Invalid client settings:{validationError}");
			}
		}


		internal static PerceptorClient CreateForRepository(ClientSettings clientSettings,
			IPerceptorRepository repository)
		{
			var taskService = new TaskLimiterService(clientSettings.MaximalNumberOfParallelRequests);
			return new PerceptorClient(data => new ContentSessionContext(taskService, repository, data));
		}
	}
}