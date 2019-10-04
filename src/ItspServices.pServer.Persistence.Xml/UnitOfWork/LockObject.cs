namespace ItspServices.pServer.Persistence.UnitOfWork
{
    internal class LockObject
    {
        public static object Lock { get; } = new object();
    }
}
