namespace Perceptor.Client.Lib.InternalModels
{
	internal readonly struct PerceptorSuccessResult
	{
		public PerceptorSuccessResult(string answer)
		{
			Answer = answer;
		}

		public string Answer { get; }
	}
}