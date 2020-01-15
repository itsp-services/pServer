using System.Threading.Tasks;

namespace ItspServices.pServer.Abstraction.UseCase
{
    public interface IUseCaseRequestHandler<in TUseCaseRequest, out TUseCaseResponse>
    {
        Task Handle(TUseCaseRequest request, IOutputPort<TUseCaseResponse> outputPort);
    }
}
