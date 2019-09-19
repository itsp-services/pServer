namespace ItspServices.pServer.Abstraction.Units
{
    public interface IAddUnitOfWork<T> : IUnitOfWork<T>
    {
        T Entity { get; }
    }
}
