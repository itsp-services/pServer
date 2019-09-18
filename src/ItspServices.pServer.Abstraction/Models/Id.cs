namespace ItspServices.pServer.Abstraction.Models
{
    public class Id
    {
        public int Value { get; set; }
        public bool IsValid { get; set; } = false;

        public static implicit operator Id(int id)
        {
            return new Id { Value = id, IsValid = true };
        }

        public static implicit operator int(Id id)
        {
            return id.Value;
        }
    }
}
