namespace ItspServices.pServer.Abstraction.Models
{
    public class ProtectedData
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public UserRegister Users { get; set; } = new UserRegister();
    }
}
