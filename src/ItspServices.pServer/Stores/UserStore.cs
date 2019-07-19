using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Stores
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        public IUserRepository UserRepository { get; }

        public UserStore(IRepositoryManager repository)
        {
            UserRepository = repository.UserRepository;
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                IUnitOfWork<User> uow = UserRepository.Add(user);
                uow.Complete();
            }
            catch (IOException exception)
            {
                return GetResultFromUnitCompleteException(exception);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                IUnitOfWork<User> uow = UserRepository.Remove(user);
                uow.Complete();
            }
            catch (IOException exception)
            {
                return GetResultFromUnitCompleteException(exception);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                IUnitOfWork<User> uow = UserRepository.Update(user);
                uow.Complete();
            }
            catch (IOException exception)
            {
                return GetResultFromUnitCompleteException(exception);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken) => 
            await Task.Run(() => UserRepository.GetById(int.Parse(userId)));


        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => 
            await Task.Run(() => UserRepository.GetUserByName(normalizedUserName));

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken) => 
            await Task.Run(() => { user.NormalizedUserName = normalizedName; });

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken) =>
            await Task.Run(() => { user.UserName = userName; });


        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken) =>
            await Task.Run(() => { user.PasswordHash = passwordHash; });
        

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null && user.PasswordHash != string.Empty);
        }

        public void Dispose()
        {
        }

        private static Task<IdentityResult> GetResultFromUnitCompleteException(IOException exception)
        {
            IdentityError error = new IdentityError();
            error.Code = exception.Source;
            error.Description = exception.Message;
            return Task.FromResult(IdentityResult.Failed(error));
        }
    }
}
