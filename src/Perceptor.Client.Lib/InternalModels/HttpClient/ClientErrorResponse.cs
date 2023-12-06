namespace Perceptor.Client.Lib.InternalModels.HttpClient
{
	internal readonly struct ClientErrorResponse : IHttpClientResponse
	{
		public ClientErrorResponse(PerceptorError errorInfo)
		{
			ErrorInfo = errorInfo;
		}

		public PerceptorError ErrorInfo { get; }
	}
}