using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels.HttpClient;

namespace Perceptor.Client.Lib.Repository.HttpClient
{
	internal interface IHttpClientFacade
	{
		Task<IHttpClientResponse> SendMessage(Func<HttpRequestMessage> messageFactory, CancellationToken cancellationToken);
	}
}