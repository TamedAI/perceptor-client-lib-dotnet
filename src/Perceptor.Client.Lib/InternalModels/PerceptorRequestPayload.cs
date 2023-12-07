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