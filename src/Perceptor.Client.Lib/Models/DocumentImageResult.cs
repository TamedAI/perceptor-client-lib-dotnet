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
	/// Response object for multi page queries
	/// </summary>
	public class DocumentImageResult
	{
		internal DocumentImageResult(int pageIndex, IReadOnlyList<InstructionWithResult> results)
		{
			PageIndex = pageIndex;
			Results = results;
		}

		/// <summary>
		/// Zero based index of original document page
		/// </summary>
		public int PageIndex { get; }

		/// <summary>
		/// List of results for the page.
		/// </summary>
		public IReadOnlyList<InstructionWithResult> Results { get; }
	}
}