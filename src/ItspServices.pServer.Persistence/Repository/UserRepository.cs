using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using ItspServices.pServer.Persistence.UnitOfWork;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.RepositoryTest")]
namespace ItspServices.pServer.Persistence.Repository
{
    class UserRepository : IUserRepository
    {
        private readonly string _filePath;

        private UserUnitOfWork _unitOfWork;

        public UserRepository(string filepath)
        {
            _filePath = filepath;
            _unitOfWork = new UserUnitOfWork(_filePath);
        }

        public IAddUnitOfWork<User> Add() 
            => new AddUserUnitOfWork(_filePath);

        public IEnumerable<User> GetAll()
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                IEnumerable<XElement> elements = from user in XDocument.Load(sr).Descendants("User")
                                                 select user;
                List<User> users = new List<User>();
                foreach(XElement element in elements)
                {
                    users.Add(UserSerializer.XElementToUser(element));
                }
                return users;
            }
        }

        public User GetById(int id)
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                XElement element = (from user in XDocument.Load(sr).Descendants("User")
                                    where (int)user.Attribute("Id") == id
                                    select user).SingleOrDefault();
                return (element != null) ? UserSerializer.XElementToUser(element) : null;
            }
        }

        public User GetUserByNormalizedName(string name)
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                XElement element = (from user in XDocument.Load(sr).Descendants("User")
                                    where user.Element("NormalizedUserName").Value == name
                                    select user).SingleOrDefault();
                return (element != null) ? UserSerializer.XElementToUser(element) : null;
            }
        }

        public IRemoveUnitOfWork<User> Remove(int key)
        {
            User user = GetById(key);
            if (user == null)
                throw new System.InvalidOperationException($"User with id {key} does not exist.");

            return new RemoveUserUnitOfWork(_filePath, user);
        }

        public IUpdateUnitOfWork<User> Update(int key)
        {
            User user = GetById(key);
            if (user == null)
                return null;

            return new UpdateUserUnitOfWork(_filePath, user);
        }
    }
}
