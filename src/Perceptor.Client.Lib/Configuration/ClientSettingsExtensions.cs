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
using System.Text;
using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Configuration
{
	internal static class ClientSettingsExtensions
	{
		internal static PerceptorRepositoryHttpClientSettings ToHttpClientSettings(this ClientSettings clientSettings)
		{
			return new PerceptorRepositoryHttpClientSettings(clientSettings.ApiKey,
				clientSettings.Url,
				new WaitTimeOut(clientSettings.WaitTimeout));
		}

		internal static string Validate(this ClientSettings clientSettings)
		{
			var errorTexts = new StringBuilder();

			if (String.IsNullOrWhiteSpace(clientSettings.ApiKey))
			{
				errorTexts.AppendLine($"invalid {nameof(clientSettings.ApiKey)}");
			}

			if (!Flurl.Url.IsValid(clientSettings.Url))
			{
				errorTexts.AppendLine($"invalid {nameof(clientSettings.Url)}");
			}

			if (clientSettings.MaximalNumberOfParallelRequests < 1)
			{
				errorTexts.AppendLine($"{nameof(clientSettings.MaximalNumberOfParallelRequests)} must be >= 1");
			}

			if (clientSettings.RetryCount < 0)
			{
				errorTexts.AppendLine($"{nameof(clientSettings.RetryCount)} must be >= 0");
			}

			return errorTexts.ToString();
		}
	}
}