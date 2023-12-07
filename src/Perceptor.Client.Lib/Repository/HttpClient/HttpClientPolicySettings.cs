using System;

namespace Perceptor.Client.Lib.Repository.HttpClient
{
	internal class HttpClientPolicySettings
	{
		public HttpClientPolicySettings(int retryCount, TimeSpan requestTimeout)
		{
			RetryCount = retryCount;
			RequestTimeout = requestTimeout;
		}

		public int RetryCount { get; }
		public TimeSpan RequestTimeout { get; }
	}
}