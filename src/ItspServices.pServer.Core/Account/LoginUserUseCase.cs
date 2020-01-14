using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Response;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.UseCase;
using ItspServices.pServer.Abstraction.UseCase.Account;

namespace ItspServices.pServer.Core.Account
{
    public class LoginUserUseCase : ILoginUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public LoginUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(LoginRequest request, IOutputPort<UseCaseResponse> outputPort)
        {
            User user = _userRepository.GetUserByNormalizedName(request.Username.ToUpper());
            if (user != null)
            {
                
            }
            outputPort.Handle(new UseCaseResponse(false, "Invalid username or password"));
        }
    }
}
