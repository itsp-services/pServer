using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Response;
using ItspServices.pServer.Abstraction.UseCase;
using ItspServices.pServer.Abstraction.UseCase.Account;

namespace ItspServices.pServer.Core.Account
{
    public class LogoutUserUseCase : ILogoutUserUseCase
    {
        private readonly ISignInManager _signInManager;

        public LogoutUserUseCase(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task Handle(LogoutRequest request, IOutputPort<UseCaseResponse> outputPort)
        {
            bool success = await _signInManager.SignOutAsync();
            outputPort.Handle(new UseCaseResponse(success, ""));
        }
    }
}
