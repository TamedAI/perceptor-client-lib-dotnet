namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct PerceptorError
	{
		public PerceptorError(string errorText)
		{
			ErrorText = errorText;
		}

		public string ErrorText { get; }

		public override string ToString()
		{
			return ErrorText;
		}
	}
}