using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ItspServices.pServer.Test.Mock.Implementation
{
    class JsonUserDbHandler : IDbUserHandler
    {
        private User ParseUserFromJToken(JToken token)
        {
            User user = new User();
            user.Id = token["id"].Value<int>();
            user.Name = token["name"].Value<string>() + ' ' + token["sname"].Value<string>();

            JArray publicKeysArr = (JArray)token["pkeys"];
            string[] publicKeys = publicKeysArr.Select(c => (string)c).ToArray();
            foreach(string s in publicKeys)
            {
                user.PublicKeys.Add(Encoding.ASCII.GetBytes(s));
            }

            return user;
        }

        public User GetUserById(int id)
        {
            JToken results;
            using (StreamReader sr = new StreamReader("C:\\Users\\AndreasS\\source\\pServer\\test\\ItspServices.pServer.Test\\Mock\\DB\\UserDataTestInput.json"))
            {
                JObject userSearch = JObject.Parse(sr.ReadToEnd());
                results = userSearch["user"];
            }
            foreach(JToken token in results)
            {
                if(token["id"].Value<int>() == id)
                    return ParseUserFromJToken(token);
            }
            return null;
        }

        public void RemoveUserById(int id)
        {
            throw new NotImplementedException();
        }

        public void RegisterUser(User u)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter("C:\\Users\\AndreasS\\source\\pServer\\test\\ItspServices.pServer.Test\\Mock\\Output\\UserDataOutput.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, u);
            }
        }
    }
}
