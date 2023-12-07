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
using Perceptor.Client.Lib.Models;

namespace Perceptor.Client.Lib.InternalModels
{
	internal class PerceptorRequestPayload
	{
		public PerceptorRequestPayload(PerceptorRequest request,
			InstructionMethod method,
			InstructionContextData contextData,
			Instruction instruction,
			IReadOnlyList<ClassificationEntry> classificationEntries)
		{
			Request = request;
			Method = method;
			ContextData = contextData;
			Instruction = instruction;
			ClassificationEntries = classificationEntries;
		}

		public PerceptorRequest Request { get; }
		public InstructionMethod Method { get; }
		public InstructionContextData ContextData { get; }

		public Instruction Instruction { get; }
		public IReadOnlyList<ClassificationEntry> ClassificationEntries { get; }
	}
}