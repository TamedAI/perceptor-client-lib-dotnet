namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct InstructionWithResponseModel
	{
		public InstructionWithResponseModel(Instruction instruction, OneOf<PerceptorSuccessResult, PerceptorError> result)
		{
			Instruction = instruction;
			Result = result;
		}

		public Instruction Instruction { get; }
		public OneOf<PerceptorSuccessResult, PerceptorError> Result { get; }
	}
}