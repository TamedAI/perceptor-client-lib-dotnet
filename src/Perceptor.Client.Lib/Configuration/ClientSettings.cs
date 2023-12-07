using System;

namespace Perceptor.Client.Lib.Configuration
{
	/// <summary>
	/// Client configuration
	/// </summary>
	public class ClientSettings
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="apiKey"><see cref="ApiKey"/></param>
		/// <param name="url"><see cref="Url"/></param>
		public ClientSettings(string apiKey, string url)
		{
			ApiKey = apiKey;
			Url = url;
		}

		/// <summary>
		/// Api key to use for authentication
		/// </summary>
		public string ApiKey { get; }
		
		/// <summary>
		/// Perceptor api url
		/// </summary>
		public string Url { get; } 
		
		/// <summary>
		/// Request timeout
		/// </summary>
		public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(60);

		/// <summary>
		/// Maximal number of parallel requests
		/// </summary>
		public int MaximalNumberOfParallelRequests { get; set; }  = 3;

		/// <summary>
		/// Number of retries in case of failed requests
		/// </summary>
		public int RetryCount { get; set; } = 3;

		
	}
}