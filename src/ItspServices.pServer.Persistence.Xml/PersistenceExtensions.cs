using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
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
