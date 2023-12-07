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
using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Models
{
	/// <summary>
	/// Response object
	/// </summary>
	public class InstructionWithResult
	{
		private InstructionWithResult(string instructionText, bool isSuccess,
			IReadOnlyDictionary<string, object> response, string errorText)
		{
			InstructionText = instructionText;
			IsSuccess = isSuccess;
			Response = response;
			ErrorText = errorText;
		}

		/// <summary>
		/// Original instruction text
		/// </summary>
		public string InstructionText { get; }

		/// <summary>
		/// true if query was successful
		/// </summary>
		public bool IsSuccess { get; }

		/// <summary>
		/// Response object, containing at least "text" element (with response text) and optionally other elements (like scores).
		/// </summary>
		public IReadOnlyDictionary<string, object> Response { get; }

		/// <summary>
		/// Error text (in case of failed query)
		/// </summary>
		public string ErrorText { get; }

		internal static InstructionWithResult Success(string instructionText,
			IReadOnlyDictionary<string, object> response) =>
			new InstructionWithResult(instructionText, true, response, String.Empty);

		private static readonly IReadOnlyDictionary<string, object> _empty = new Dictionary<string, object>();

		internal static InstructionWithResult Error(string instructionResult, string errorText) =>
			new InstructionWithResult(instructionResult, false, _empty, errorText);

		internal static IReadOnlyList<InstructionWithResult> FromResponseModels(
			IEnumerable<InstructionWithResponseModel> models)
			=> models.Select(FromResponseModel).ToArray();

		private static InstructionWithResult FromResponseModel(InstructionWithResponseModel model) =>
			model.Result.Match(
				success =>
				{
					IReadOnlyDictionary<string, object> resp =
						StructuredResponseParser.ParseStructuredText(success.Answer);
					return Success(model.Instruction.Content, resp);
				},
				error => Error(model.Instruction.Content, error.ErrorText));
	}
}