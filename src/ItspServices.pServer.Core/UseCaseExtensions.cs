using Microsoft.Extensions.DependencyInjection;
using ItspServices.pServer.Abstraction.UseCase.Account;
using ItspServices.pServer.Core.Account;

namespace ItspServices.pServer.Core
{
    public static class UseCaseExtensions
    {
        public static void AddAuthUseCases(this IServiceCollection services)
        {
            services.AddTransient(typeof(ILoginUserUseCase), typeof(LoginUserUseCase));
            services.AddTransient(typeof(ILogoutUserUseCase), typeof(LogoutUserUseCase));
            services.AddTransient(typeof(IRegisterUserUseCase), typeof(RegisterUserUseCase));
        }
    }
}
