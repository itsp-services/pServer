using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Response;

namespace ItspServices.pServer.Abstraction.UseCase.Account
{
    public interface IRegisterUserUseCase : IUseCaseRequestHandler<RegisterRequest, UseCaseResponse>
    {
    }
}
