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

		public async Task<IHttpClientResponse> SendMessage(Func<HttpRequestMessage> messageFactory,
			CancellationToken cancellationToken)
		{
			using HttpRequestMessage message = messageFactory();
			HttpResponseMessage responseMessage = await _httpClient.SendAsync(message,
				HttpCompletionOption.ResponseContentRead,
				cancellationToken);
			return new ClientHttpMessageResponse(responseMessage);
		}
	}
}