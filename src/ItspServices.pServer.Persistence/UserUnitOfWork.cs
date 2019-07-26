using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ItspServices.pServer.Persistence
{
    class UserUnitOfWork : IUnitOfWork<User>
    {
        private string _filePath;

        internal TransactionRecord<User> TransactionRecord { get; }

        public UserUnitOfWork(string filePath)
        {
            _filePath = filePath;
            TransactionRecord = new TransactionRecord<User>();
        }

        public void Complete()
        {
            CommitAdd();
            CommitRemove();
            CommitUpdate();
            TransactionRecord.Clear();
        }

        public void Dispose()
        {
            Rollback();
        }

        public void Rollback()
        {
            TransactionRecord.Clear();
        }

        private void CommitUpdate()
        {
            IEnumerable<User> stagedUsers = TransactionRecord.StagedEntitiesAsEnumerable(TransactionActions.UPDATE);
            if (stagedUsers == null)
                return;

            XDocument document;
            using (StreamReader sr = new StreamReader(_filePath))
            {
                document = XDocument.Load(sr);

                foreach (User user in stagedUsers)
                {
                    XElement elementToUpdate = (from userEntry in document.Descendants("User")
                                                where (int)userEntry.Attribute("Id") == user.Id
                                                select userEntry).SingleOrDefault();
                    elementToUpdate.SetElementValue("UserName", user.UserName);
                    elementToUpdate.SetElementValue("NormalizedUserName", user.NormalizedUserName);
                    elementToUpdate.SetElementValue("PasswordHash", user.PasswordHash);
                    elementToUpdate.Element("PublicKeys").Descendants().Remove();
                    elementToUpdate.Element("PublicKeys").Add(from key in user.PublicKeys
                                                              select new XElement(
                                                                  "PublicKey",
                                                                  Encoding.UTF8.GetString(key.KeyData),
                                                                  new XAttribute("Id", key.Id),
                                                                  new XAttribute("Flag", (int)key.Flag)
                                                                  ));

                }
            }

            using (StreamWriter sw = new StreamWriter(_filePath))
                document.Save(sw);
        }

        private void CommitRemove()
        {
            IEnumerable<User> stagedUsers = TransactionRecord.StagedEntitiesAsEnumerable(TransactionActions.REMOVE);
            if (stagedUsers == null)
                return;

            XDocument document;
            using (StreamReader sr = new StreamReader(_filePath))
            {
                document = XDocument.Load(sr);

                foreach (User user in stagedUsers)
                {
                    document.Root.Elements().Where(x => (int)x.Attribute("Id") == user.Id).Remove();
                }
            }

            using (StreamWriter sw = new StreamWriter(_filePath))
                document.Save(sw);
        }

        private void CommitAdd()
        {
            IEnumerable<User> stagedUsers = TransactionRecord.StagedEntitiesAsEnumerable(TransactionActions.ADD);
            if (stagedUsers == null)
                return;

            XDocument document;
            using (StreamReader sr = new StreamReader(_filePath))
            {
                document = XDocument.Load(sr);

                foreach(User user in stagedUsers)
                {
                    document.Element("Users").Add(UserSerializer.UserToXElement(user));
                }
            }

            using (StreamWriter sw = new StreamWriter(_filePath))
                document.Save(sw);
        }
    }
}
