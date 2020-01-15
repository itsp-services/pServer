using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Response;
using ItspServices.pServer.Abstraction.UseCase;
using ItspServices.pServer.Abstraction.UseCase.Account;

namespace ItspServices.pServer.Core.Account
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserManager _userManager;

        public RegisterUserUseCase(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(RegisterRequest request, IOutputPort<UseCaseResponse> outputPort)
        {
            bool success = await _userManager.CreateUserAsync(request.Username, request.Password);
            outputPort.Handle(new UseCaseResponse(success,
                                                  success ? $"User {request.Username} created." : "User could not be created."));
        }
    }
}
