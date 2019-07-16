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
        public IUnitOfWork Add()
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

        public IUnitOfWork Remove()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork Update(int id)
        {
            throw new NotImplementedException();
        }
    }
}
