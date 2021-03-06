﻿using ItspServices.pServer.Abstraction.Authorizer;
using ItspServices.pServer.Abstraction.Models;

namespace ItspServices.pServer.Authorization
{
    public class UserDataAuthorizer : IUserDataAuthorizer
    {
        public User User { get; set; }
        public ProtectedData Data { get; set; }

        public UserDataAuthorizer(User user, ProtectedData data)
        {
            User = user;
            Data = data;
        }

        public bool Authorize()
        {
            return false;
        }
    }
}
