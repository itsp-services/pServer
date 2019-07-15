using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Stores
{
    public class UserStore : IUserStore<User>
    {
        public IUserRepository UserRepository { get; }

        public UserStore(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            using (IUserUnit unit = (IUserUnit) UserRepository.Add())
            {
                unit.User.Id = user.Id;
                unit.User.UserName = user.UserName;
                unit.User.NormalizedUserName = user.NormalizedUserName;

                unit.Complete();
                return Task.FromResult(IdentityResult.Success);
            }
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken) => 
            await Task.Run(() => UserRepository.GetById(userId));


        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => 
            await Task.Run(() => UserRepository.GetUserByName(normalizedUserName));

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken) => 
            await Task.Run(() => { user.NormalizedUserName = normalizedName; });

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken) =>
            await Task.Run(() => { user.UserName = userName; });

        public void Dispose()
        {
        }
    }
}
