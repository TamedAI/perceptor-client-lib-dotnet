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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Services;

namespace Perceptor.Client.Lib.Repository
{
	internal static class RepositoryHelpers
	{
		public static string MapToPayloadString(PerceptorRequestPayload payload, WaitTimeOut waitTimeout)
		{
			IReadOnlyDictionary<string, string> parametersDictionary = payload.Request.Parameters;
			var parametersWithReturnScores = parametersDictionary
				.Select(x => x)
				.Append(new KeyValuePair<string, string>("returnScores",
					payload.Request.ReturnScores ? "true" : "false"))
				.ToDictionary(x => x.Key, x => x.Value);

			var dictionary = new Dictionary<string, object>()
			{
				{ "flavor", payload.Request.Flavor },
				{ "contextType", payload.ContextData.ContextType },
				{ "context", payload.ContextData.Content },
				{ "instruction", payload.Instruction.Content },
				{ "waitTimeout", waitTimeout.TimeSpan.TotalSeconds },
				{ "params", parametersWithReturnScores },
			};
			if (payload.Method == InstructionMethod.Classify)
			{
				dictionary.Add("classes", payload.ClassificationEntries.Select(c => c.Value));
			}

			return SerializationService.Serialize(dictionary);
		}

		public static string MapBadRequestContentString(this string content)
		{
			const string detailKey = "detail";

			try
			{
				var dict = SerializationService.Deserialize<Dictionary<string, object>>(content);

				if (dict.TryGetValue(detailKey, out object val))
				{
					return val?.ToString() ?? String.Empty;
				}

				return content;
			}
			catch (JsonException)
			{
				return content;
			}
		}

		internal static async Task<string> GetResponseTextContent(this HttpResponseMessage responseMessage)
		{
			return await responseMessage.Content.ReadAsStringAsync();
		}
	}
}