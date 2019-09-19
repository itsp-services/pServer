namespace ItspServices.pServer.Abstraction.Units
{
    public interface IRemoveUnitOfWork<Tentity, Tkey> : IUnitOfWork<Tentity>
    {
        Tkey Id { get; }
        Tentity Entity { get; }
    }
}
