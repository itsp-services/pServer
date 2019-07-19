using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Test.Mock.Repository
{
    class MockRoleRepository : IRoleRepository
    {
        public IUnitOfWork<Role> Add(Role entity)
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

        public IUnitOfWork<Role> Remove(Role entity)
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork<Role> Update(Role entity)
        {
            throw new NotImplementedException();
        }
    }
}
