using System.Net.Http;

namespace Perceptor.Client.Lib.InternalModels.HttpClient
{
	internal readonly struct ClientHttpMessageResponse : IHttpClientResponse
	{
		public ClientHttpMessageResponse(HttpResponseMessage message)
		{
			Message = message;
		}

		public HttpResponseMessage Message { get; }
	}
}