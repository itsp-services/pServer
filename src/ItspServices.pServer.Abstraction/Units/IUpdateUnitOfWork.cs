namespace ItspServices.pServer.Abstraction.Units
{
    public interface IUpdateUnitOfWork<T> : IUnitOfWork<T>
    {
        int? Id { get; set; }
        T Entity { get; }
    }
}
