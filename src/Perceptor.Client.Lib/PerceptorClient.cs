using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels;
using Perceptor.Client.Lib.Models;
using Perceptor.Client.Lib.Repository;

namespace Perceptor.Client.Lib
{
	/// <summary>
	/// Perceptor client
	/// </summary>
	public class PerceptorClient
	{
		private readonly Func<InstructionContextData, ContentSessionContext> _createContextFunc;

		internal PerceptorClient(Func<InstructionContextData, ContentSessionContext> factoryFunc)
		{
			_createContextFunc = factoryFunc;
		}


		internal Task<IReadOnlyList<InstructionWithResult>> AskQuestionInContext(
			InstructionContextData contextData,
			PerceptorRequest request,
			IEnumerable<string> textInstructions,
			CancellationToken cancellationToken) =>
			AskInContext(contextData, request, InstructionMethod.Question, textInstructions, Enumerable.Empty<string>(), cancellationToken);

		internal Task<InstructionWithResult> AskTableInContext(
			InstructionContextData contextData,
			PerceptorRequest request,
			string textInstruction,
			CancellationToken cancellationToken) =>
			AskSingleInstructionInContext(
				InstructionMethod.Table,
				contextData,
				request,
				textInstruction, 
				Enumerable.Empty<string>(), 
				cancellationToken
			);

		
		internal Task<InstructionWithResult> AskClassifyInContext(
			InstructionContextData contextData,
			PerceptorRequest request,
			string textInstruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken)=>
			AskSingleInstructionInContext(
				InstructionMethod.Classify,
				contextData,
				request,
				textInstruction, 
				classes, 
				cancellationToken
			);

		private async Task<InstructionWithResult> AskSingleInstructionInContext(
			InstructionMethod method,
			InstructionContextData contextData,
			PerceptorRequest request,
			string textInstruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken)
		{
			var res = await AskInContext(contextData, request, method,
				new[] { textInstruction },
				classes,
				cancellationToken);

			return res.Any() ? res.First() : InstructionWithResult.Error(textInstruction, ErrorConstants.UnknownError.ErrorText);
		}
		
		private async Task<IReadOnlyList<InstructionWithResult>> AskInContext(InstructionContextData contextData,
			PerceptorRequest request,
			InstructionMethod method,
			IEnumerable<string> textInstructions,
			IEnumerable<string> classes,
			CancellationToken cancellationToken)
		{
			Instruction[] mappedInstructions = textInstructions.Select(t => new Instruction(t))
				.ToArray();
			ClassificationEntry[] mappedClasses = classes.Select(t => new ClassificationEntry(t)).ToArray();

			var responseModel = await _createContextFunc(contextData).ProcessInstructionRequest(request,
				method,
				mappedInstructions,
				mappedClasses,
				cancellationToken
			);

			return InstructionWithResult.FromResponseModels(responseModel);
		}

		internal async Task<IReadOnlyList<DocumentImageResult>> AskTableInMultipleContexts(
			IEnumerable<InstructionContextData> contexts,
			PerceptorRequest request,
			string instruction,
			IEnumerable<string> classes,
			CancellationToken cancellationToken) =>
			await AskInMultipleContexts(contexts,
				request,
				InstructionMethod.Table,
				new[] { instruction },
				classes,
				cancellationToken);

		internal async Task<IReadOnlyList<DocumentImageResult>> AskInMultipleContexts(
			IEnumerable<InstructionContextData> contexts,
			PerceptorRequest request,
			InstructionMethod method,
			IEnumerable<string> textInstructions,
			IEnumerable<string> classes,
			CancellationToken cancellationToken)
		{
			List<string> instructionsAsList = textInstructions.ToList();
			Task<DocumentImageResult>[] contextTasks = contexts
				.Select((ctx, i) => ProcessContext(i, ctx))
				.ToArray();

			await Task.WhenAll(contextTasks);

			return contextTasks
				.Select(ct => ct.Result)
				.ToList();

			async Task<DocumentImageResult> ProcessContext(int index, InstructionContextData contextData)
			{
				var response = await AskInContext(contextData, request, method, instructionsAsList, 
					classes,
					cancellationToken);
				return new DocumentImageResult(index, response);
			}
		}
	}
}