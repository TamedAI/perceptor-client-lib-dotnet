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

using System.Collections.Generic;

namespace Perceptor.Client.Lib.Models
{
	/// <summary>
	/// Request information
	/// </summary>
	public class PerceptorRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="flavor"></param>
		/// <param name="parameters"></param>
		/// <param name="returnScores"></param>
		public PerceptorRequest(string flavor, IReadOnlyDictionary<string, string> parameters, bool returnScores)
		{
			Parameters = parameters;
			ReturnScores = returnScores;
			Flavor = flavor;
		}

		/// <summary>
		/// Flavor
		/// </summary>
		public string Flavor { get; }

		/// <summary>
		/// Detailed request parameters
		/// </summary>
		public IReadOnlyDictionary<string, string> Parameters { get; }

		/// <summary>
		/// true if scores are to be returned
		/// </summary>
		public bool ReturnScores { get; }

		/// <summary>
		/// Creates another instance where <see cref="ReturnScores"/> is set to true
		/// </summary>
		/// <returns></returns>
		public PerceptorRequest WithReturnScores()
		{
			return ReturnScores ? this : new PerceptorRequest(Flavor, Parameters, true);
		}

		private static readonly IReadOnlyDictionary<string, string> _emptyParameters = new Dictionary<string, string>();

		/// <summary>
		/// Creates an instance with default parameters and the specified <paramref name="flavor"/>
		/// </summary>
		/// <param name="flavor"></param>
		/// <returns></returns>
		public static PerceptorRequest WithFlavor(string flavor)
		{
			return new PerceptorRequest(flavor, _emptyParameters, false);
		}
	}
}