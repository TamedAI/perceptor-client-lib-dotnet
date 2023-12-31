﻿// /*
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
using Polly;
using Polly.Retry;
using Polly.Timeout;

namespace Perceptor.Client.Lib.Repository.HttpClient
{
	internal class HttpClientFacadePollyDecorator : IHttpClientFacade
	{
		private readonly IHttpClientFacade _decoree;
		private readonly AsyncPolicy _policy;

		public HttpClientFacadePollyDecorator(HttpClientPolicySettings settings, IHttpClientFacade decoree)
		{
			_decoree = decoree;
			_policy = BuildPolicy(settings);
		}

		private static AsyncPolicy BuildPolicy(HttpClientPolicySettings settings)
		{
			Func<Exception, bool> excPredicate = exc => exc is TimeoutRejectedException || exc is TaskCanceledException;

			AsyncRetryPolicy retryPolicy = Policy.Handle(excPredicate)
				.RetryAsync(settings.RetryCount);

			AsyncTimeoutPolicy timeoutPolicy =
				Policy.TimeoutAsync(settings.RequestTimeout, TimeoutStrategy.Pessimistic);
			return Policy.WrapAsync(retryPolicy, timeoutPolicy);
		}

		public async Task<IHttpClientResponse> SendMessage(Func<HttpRequestMessage> messageFactory,
			CancellationToken cancellationToken)
		{
			PolicyResult<IHttpClientResponse> policyResult = await _policy.ExecuteAndCaptureAsync(
				ct => _decoree.SendMessage(messageFactory, ct),
				cancellationToken);
			return policyResult.Outcome switch
			{
				OutcomeType.Failure => MapRequestException(policyResult.FinalException),
				_ => policyResult.Result
			};
		}

		private static readonly ClientErrorResponse _requestCancelledResponse =
			new ClientErrorResponse(ErrorConstants.RequestCancelledError);

		private static readonly ClientErrorResponse _clientTimeoutResponse =
			new ClientErrorResponse(ErrorConstants.TimeOutError);

		private static readonly ClientErrorResponse _unknownErrorResponse =
			new ClientErrorResponse(ErrorConstants.UnknownError);

		private static ClientErrorResponse MapRequestException(Exception exception)
		{
			return exception switch
			{
				TaskCanceledException _ => _requestCancelledResponse,
				TimeoutRejectedException _ => _clientTimeoutResponse,
				_ => _unknownErrorResponse
			};
		}
	}
}