namespace ItspServices.pServer.Abstraction.Units
{
    interface IUpdateUnitOfWork<T> : IUnitOfWork<T>
    {
        int Id { get; set; }
        T Entity { get; }
    }
}
