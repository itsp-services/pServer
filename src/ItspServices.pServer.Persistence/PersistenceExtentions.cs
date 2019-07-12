using Microsoft.Extensions.DependencyInjection;
using ItspServices.pServer.Abstraction.Repository;

namespace ItspServices.pServer.Persistence
{
    public static class PersistenceExtentions
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            //services.AddTransient(typeof(IRepository), typeof(Repository.Repository));
        }
    }
}
