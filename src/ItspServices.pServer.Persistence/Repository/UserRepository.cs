using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.RepositoryTest")]
namespace ItspServices.pServer.Persistence.Repository
{
    class UserRepository : IUserRepository
    {
        private readonly string _filePath;

        private UnitOfWork _unitOfWork;

        public UserRepository(string filepath)
        {
            _filePath = filepath;
            _unitOfWork = new UnitOfWork(_filePath);
        }

        public IUnitOfWork<User> Add(User entity)
        {
            entity.Id = GetAvailableId();
            _unitOfWork.TransactionRecord.Add(entity, TransactionActions.ADD);
            return _unitOfWork;
        }

        public IEnumerable<User> GetAll()
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                IEnumerable<XElement> elements = from user in XDocument.Load(sr).Descendants("User")
                                                 select user;
                List<User> users = new List<User>();
                foreach(XElement element in elements)
                {
                    users.Add(XElementToUser(element));
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
                return (element != null) ? XElementToUser(element) : null;
            }
        }

        public User GetUserByNormalizedName(string name)
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                XElement element = (from user in XDocument.Load(sr).Descendants("User")
                                    where user.Element("NormalizedUserName").Value == name
                                    select user).SingleOrDefault();
                return (element != null) ? XElementToUser(element) : null;
            }
        }

        public IUnitOfWork<User> Remove(User entity)
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork<User> Update(User entity)
        {
            throw new NotImplementedException();
        }

        private int GetAvailableId()
        {
            return 0;
        }

        private static User XElementToUser(XElement element)
        {
            User user = new User();
            user.Id = (int)element.Attribute("Id");
            user.UserName = element.Element("UserName").Value;
            user.NormalizedUserName = element.Element("NormalizedUserName").Value;
            user.PasswordHash = element.Element("PasswordHash").Value;
            user.PublicKeys = (from key in element.Element("PublicKeys").Descendants("PublicKey")
                               select Encoding.UTF8.GetBytes(key.Value)).ToList();
            return user;
        }
    }
}
