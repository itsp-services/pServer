namespace ItspServices.pServer.Abstraction.Units
{
    public interface IUpdateUnitOfWork<Tentity, Tkey> : IUnitOfWork<Tentity>
    {
        Tkey Id { get; }
        Tentity Entity { get; }
    }
}
