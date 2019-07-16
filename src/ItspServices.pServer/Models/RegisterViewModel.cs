using System.ComponentModel.DataAnnotations;

namespace ItspServices.pServer.Models
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "Username")]
        public string Username { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "Password")]
        public string Password { get; set; }
        [Required, DataType(DataType.Password), Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}
