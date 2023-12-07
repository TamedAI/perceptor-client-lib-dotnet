using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Repository
{
	internal static class ErrorConstants
	{
		public static PerceptorError InvalidApiKey => new PerceptorError("invalid api key");
		public static PerceptorError UnknownError => new PerceptorError("Unknown error");
		public static PerceptorError TimeOutError => new PerceptorError("Request timeout");
		public static PerceptorError RequestCancelledError => new PerceptorError("Request cancelled");
	}
}