using System.Threading;
using System.Threading.Tasks;
using Perceptor.Client.Lib.InternalModels;

namespace Perceptor.Client.Lib.Repository
{
	internal interface IPerceptorRepository
	{
		Task<OneOf<PerceptorSuccessResult, PerceptorError>> SendInstruction(PerceptorRequestPayload perceptorRequestPayload,
			CancellationToken cancellationToken);
	}
}