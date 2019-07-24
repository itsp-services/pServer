using ItspServices.pServer.Abstraction.Models;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ItspServices.pServer.Persistence
{
    internal static class UserSerializer
    {
        public static XElement UserToXElement(User user)
        {
            return new XElement("User", new XAttribute("Id", user.Id),
                        new XElement("UserName", user.UserName),
                        new XElement("NormalizedUserName", user.NormalizedUserName),
                        new XElement("PasswordHash", user.PasswordHash),
                        new XElement("PublicKeys", 
                            from key in user.PublicKeys
                            select new XElement("PublicKey", key.GetKeyAsString())
                                    )
                                );
        }

        public static User XElementToUser(XElement element)
        {
            User user = new User();
            user.Id = (int)element.Attribute("Id");
            user.UserName = element.Element("UserName").Value;
            user.NormalizedUserName = element.Element("NormalizedUserName").Value;
            user.PasswordHash = element.Element("PasswordHash").Value;
            user.PublicKeys = (from key in element.Element("PublicKeys").Descendants("PublicKey")
                               select new Key(key.Value)).ToList();
            return user;
        }
    }
}
