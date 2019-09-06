namespace ItspServices.pServer.Abstraction.Units
{
    public interface IRemoveUnitOfWork<T> : IUnitOfWork<T>
    {
        int? Id { get; set; }
    }
}
