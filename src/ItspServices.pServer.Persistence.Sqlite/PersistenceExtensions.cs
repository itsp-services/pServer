using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Persistence.Sqlite.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ItspServices.pServer.Persistence.Sqlite
{
    public static class PersistenceExtensions
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IRepositoryManager), typeof(RepositoryManager));
        }
    }
}
