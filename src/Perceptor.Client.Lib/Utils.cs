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
using Perceptor.Client.Lib.Models;

namespace Perceptor.Client.Lib
{
	public static class Utils
	{
		/// <summary>
		/// Groups the given <paramref name="inputList"/> by instruction text.
		/// </summary>
		/// <param name="inputList">List to group</param>
		/// <returns>List of <see cref="InstructionWithPageResult"/></returns>
		public static IReadOnlyList<InstructionWithPageResult> GroupByInstruction(
			this IReadOnlyList<DocumentImageResult> inputList)
		{
			if (!inputList.Any())
			{
				return Array.Empty<InstructionWithPageResult>();
			}

			IEnumerable<string> instructionsToProcess = inputList[0]
				.Results.Select(r => r.InstructionText);

			return instructionsToProcess.Select(i =>
				new InstructionWithPageResult(i, inputList.SelectMany(x => ToDocPageWithResult(i, x)).ToList())
			).ToList();

			IEnumerable<DocumentPageWithResult> ToDocPageWithResult(string instructionText,
				DocumentImageResult documentImageResult)
			{
				return documentImageResult.Results.Where(r => r.InstructionText == instructionText)
					.Take(1)
					.Select(x =>
						new DocumentPageWithResult(documentImageResult.PageIndex, x.IsSuccess, x.Response,
							x.ErrorText));
			}
		}
	}
}