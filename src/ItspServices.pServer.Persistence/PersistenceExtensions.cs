using System;
using System.Collections.Generic;
using System.Text;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace ItspServices.pServer.Persistence
{
    public static class PersistenceExtensions
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IRepositoryManager), typeof(RepositoryManager));
        }
    }
}
