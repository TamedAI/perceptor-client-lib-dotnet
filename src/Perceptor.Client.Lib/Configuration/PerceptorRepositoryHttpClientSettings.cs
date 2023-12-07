using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Configuration
{
	internal class PerceptorRepositoryHttpClientSettings
	{
		public PerceptorRepositoryHttpClientSettings(string apiKey, string url, WaitTimeOut waitTimeout)
		{
			ApiKey = apiKey;
			Url = url;
			WaitTimeout = waitTimeout;
		}

		public string ApiKey { get;  }
		public string Url { get;  }
		public WaitTimeOut WaitTimeout { get; }
	}
}