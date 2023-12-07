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

namespace Perceptor.Client.Lib.Configuration
{
	/// <summary>
	/// Client configuration
	/// </summary>
	public class ClientSettings
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="apiKey"><see cref="ApiKey"/></param>
		/// <param name="url"><see cref="Url"/></param>
		public ClientSettings(string apiKey, string url)
		{
			ApiKey = apiKey;
			Url = url;
		}

		/// <summary>
		/// Api key to use for authentication
		/// </summary>
		public string ApiKey { get; }

		/// <summary>
		/// Perceptor api url
		/// </summary>
		public string Url { get; }

		/// <summary>
		/// Request timeout
		/// </summary>
		public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(60);

		/// <summary>
		/// Maximal number of parallel requests
		/// </summary>
		public int MaximalNumberOfParallelRequests { get; set; } = 3;

		/// <summary>
		/// Number of retries in case of failed requests
		/// </summary>
		public int RetryCount { get; set; } = 3;
	}
}