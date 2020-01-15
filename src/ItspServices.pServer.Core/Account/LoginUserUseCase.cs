using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Response;
using ItspServices.pServer.Abstraction.UseCase;
using ItspServices.pServer.Abstraction.UseCase.Account;

namespace ItspServices.pServer.Core.Account
{
    public class LoginUserUseCase : ILoginUserUseCase
    {
        private readonly ISignInManager _signInManager;

        public LoginUserUseCase(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task Handle(LoginRequest request, IOutputPort<UseCaseResponse> outputPort)
        {
            bool success = await _signInManager.PasswordSignInAsync(request.Username, request.Password);
            outputPort.Handle(new UseCaseResponse(success, success ? "" : "Invalid username or password"));
        }
    }
}
