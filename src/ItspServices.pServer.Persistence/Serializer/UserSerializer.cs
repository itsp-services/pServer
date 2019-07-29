using ItspServices.pServer.Abstraction.Models;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static ItspServices.pServer.Abstraction.Models.Key;

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
                        new XElement("Role", user.Role),
                        new XElement("PublicKeys", 
                            from key in user.PublicKeys
                            select new XElement(
                                        "PublicKey", 
                                        Encoding.UTF8.GetString(key.KeyData), 
                                        new XAttribute("Id", key.Id),
                                        new XAttribute("Flag", (int)key.Flag)
                                        )
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
            user.Role = element.Element("Role").Value;
            user.PublicKeys = (from key in element.Element("PublicKeys").Descendants("PublicKey")
                               select new Key() {
                                   Id = (int)key.Attribute("Id"),
                                   KeyData = Encoding.UTF8.GetBytes(key.Value),
                                   Flag = (KeyFlag)(int)key.Attribute("Flag")
                               }).ToList();
            return user;
        }
    }
}
