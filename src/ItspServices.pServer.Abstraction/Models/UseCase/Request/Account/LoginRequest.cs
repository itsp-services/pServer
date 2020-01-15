﻿namespace ItspServices.pServer.Abstraction.Models.UseCase.Request.Account
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
