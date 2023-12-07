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
	public class DocumentPageWithResult
	{
		public DocumentPageWithResult(int pageIndex, bool isSuccess, IReadOnlyDictionary<string, object> response,
			string errorText)
		{
			PageIndex = pageIndex;
			IsSuccess = isSuccess;
			Response = response;
			ErrorText = errorText;
		}

		/// <summary>
		/// Zero based index of original document page
		/// </summary>
		public int PageIndex { get; }

		public bool IsSuccess { get; }

		/// <summary>
		/// Response object, containing at least "text" element (with response text) and optionally other elements (like scores).
		/// </summary>
		public IReadOnlyDictionary<string, object> Response { get; }

		/// <summary>
		/// Error text (in case of failed query)
		/// </summary>
		public string ErrorText { get; }
	}
}