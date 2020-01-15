namespace ItspServices.pServer.Abstraction.Models.UseCase.Request.Account
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public RegisterRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
