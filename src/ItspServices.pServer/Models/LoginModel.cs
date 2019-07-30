using System.ComponentModel.DataAnnotations;

namespace ItspServices.pServer.Models
{
    public class LoginModel
    {
        [Required, Display(Name = "Username")]
        public string Username { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string Password { get; set; }
    }
}