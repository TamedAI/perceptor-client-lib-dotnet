// /*
// Copyright 2023 TamedAI GmbH
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

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