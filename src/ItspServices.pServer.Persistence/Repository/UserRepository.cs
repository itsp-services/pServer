using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private UserUnitOfWork _unitOfWork;

        public UserRepository(string filepath)
        {
            _filePath = filepath;
            _unitOfWork = new UserUnitOfWork(_filePath);
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

        public IUnitOfWork<User> Remove(User entity)
        {
            _unitOfWork.TransactionRecord.Add(entity, TransactionActions.REMOVE);
            return _unitOfWork;
        }

        public IUnitOfWork<User> Update(User entity)
        {
            _unitOfWork.TransactionRecord.Add(entity, TransactionActions.UPDATE);
            return _unitOfWork;
        }

        private int GetAvailableId()
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                IEnumerable<int> ids = from user in XDocument.Load(sr).Descendants("User")
                                       orderby (int)user.Attribute("Id") ascending
                                       select (int)user.Attribute("Id");
                int availableId = 0;
                foreach (int id in ids)
                {
                    if (id != availableId)
                        break;
                    availableId++;
                }

                return availableId;
            }
        }
    }
}
