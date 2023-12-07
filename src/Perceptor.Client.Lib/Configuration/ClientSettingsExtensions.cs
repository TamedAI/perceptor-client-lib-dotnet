using System;
using System.Text;
using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Configuration
{
	internal static class ClientSettingsExtensions
	{
		internal static PerceptorRepositoryHttpClientSettings ToHttpClientSettings(this ClientSettings clientSettings)
		{
			return new PerceptorRepositoryHttpClientSettings(clientSettings.ApiKey,
				clientSettings.Url,
				new WaitTimeOut(clientSettings.WaitTimeout));
		}

		internal static string Validate(this ClientSettings clientSettings)
		{
			var errorTexts = new StringBuilder();
			
			if (String.IsNullOrWhiteSpace(clientSettings.ApiKey))
			{
				errorTexts.AppendLine($"invalid {nameof(clientSettings.ApiKey)}");
			}

			if (!Flurl.Url.IsValid(clientSettings.Url))
			{
				errorTexts.AppendLine($"invalid {nameof(clientSettings.Url)}");
			}

			if (clientSettings.MaximalNumberOfParallelRequests < 1)
			{
				errorTexts.AppendLine($"{nameof(clientSettings.MaximalNumberOfParallelRequests)} must be >= 1");
			}

			if (clientSettings.RetryCount < 0)
			{
				errorTexts.AppendLine($"{nameof(clientSettings.RetryCount)} must be >= 0");
			}

			return errorTexts.ToString();
		}
	}
}