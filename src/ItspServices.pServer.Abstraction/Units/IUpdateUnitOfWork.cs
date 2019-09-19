namespace ItspServices.pServer.Abstraction.Units
{
    public interface IUpdateUnitOfWork<T> : IUnitOfWork<T>
    {
        T Entity { get; }
    }
}
