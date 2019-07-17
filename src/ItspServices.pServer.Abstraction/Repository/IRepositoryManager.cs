namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
    }
}
