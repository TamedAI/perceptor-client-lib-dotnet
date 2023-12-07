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
using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Repository;
using Perceptor.Client.Lib.Services;

namespace Perceptor.Client.Lib
{
	internal class ContentSessionContext
	{
		private readonly IPerceptorRepository _repository;
		private readonly InstructionContextData _instructionContextData;
		private readonly TaskLimiterService _taskService;

		public ContentSessionContext(TaskLimiterService taskService, IPerceptorRepository repository,
			InstructionContextData instructionContextData)
		{
			_repository = repository;
			_instructionContextData = instructionContextData;
			_taskService = taskService;
		}

		public async Task<InstructionWithResponseModel[]> ProcessInstructionRequest(PerceptorRequest request,
			InstructionMethod method,
			IReadOnlyList<Instruction> instructions,
			IReadOnlyList<ClassificationEntry> classes,
			CancellationToken cancellationToken)
		{
			if (!instructions.Any())
			{
				return Array.Empty<InstructionWithResponseModel>();
			}

			if (method == InstructionMethod.Classify && classes.Count < 2)
			{
				throw new ArgumentException("number of classes must be > 1");
			}

			var instructionTasks = instructions
				.Select(ProcessSingleInstruction)
				.ToArray();

			await Task.WhenAll(instructionTasks);

			return instructionTasks.Select(t => t.Result)
				.ToArray();

			async Task<InstructionWithResponseModel> ProcessSingleInstruction(Instruction instruction)
			{
				return await _taskService.Exec(async () =>
				{
					OneOf<PerceptorSuccessResult, PerceptorError> result =
						await ProcessInstruction(request, method, instruction, classes, cancellationToken);
					return new InstructionWithResponseModel(instruction, result);
				}, cancellationToken);
			}
		}

		private async Task<OneOf<PerceptorSuccessResult, PerceptorError>> ProcessInstruction(PerceptorRequest request,
			InstructionMethod method,
			Instruction instruction,
			IReadOnlyList<ClassificationEntry> classes,
			CancellationToken cancellationToken)
		{
			var payload = new PerceptorRequestPayload(request,
				method,
				_instructionContextData,
				instruction,
				classes);

			var result = await _repository.SendInstruction(payload, cancellationToken);
			return result;
		}
	}
}