using System;
using System.Collections.Generic;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

namespace ItspServices.pServer.Persistence.Repository
{
    class RoleRepository : IRoleRepository
    {
        public IUnitOfWork<Role> Add()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> GetAll()
        {
            throw new NotImplementedException();
        }

        public Role GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Role GetRoleByName(string name)
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork<Role> Remove()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork<Role> Update()
        {
            throw new NotImplementedException();
        }
    }
}
