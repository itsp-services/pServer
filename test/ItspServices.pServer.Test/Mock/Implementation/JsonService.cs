using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Models;
using Newtonsoft.Json.Linq;

namespace ItspServices.pServer.Test.Mock.Implementation
{
    class JsonService : IDataService
    {
        private const string _JsonUserDataPath = "C:\\Users\\AndreasS\\source\\pServer\\test\\ItspServices.pServer.Test\\Mock\\DB\\UserDataTestInput.json";

        public List<T> GetAll<T>()
        {
            if(typeof(T) == typeof(User))
            {
                
                JToken results;
                using (StreamReader sr = new StreamReader(_JsonUserDataPath))
                {
                    JObject userData = JObject.Parse(sr.ReadToEnd());
                    results = userData["user"];
                }
                List<User> list = new List<User>();
                foreach(JToken token in results)
                {
                    list.Add(ParseUserFromJToken(token));
                }
            }
            return null;
        }

        public T GetById<T>(long id)
        {
            throw new NotImplementedException();
        }

        private User ParseUserFromJToken(JToken token)
        {
            User user = new User();
            user.Id = token["id"].Value<int>();
            user.Name = token["name"].Value<string>() + ' ' + token["sname"].Value<string>();

            JArray publicKeysArr = (JArray)token["pkeys"];
            string[] publicKeys = publicKeysArr.Select(c => (string)c).ToArray();
            foreach (string s in publicKeys)
            {
                user.PublicKeys.Add(Encoding.ASCII.GetBytes(s));
            }

            return user;
        }
    }
}
