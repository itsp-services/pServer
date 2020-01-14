namespace ItspServices.pServer.Abstraction.UseCase
{
    public interface IOutputPort<in TUseCaseResponse>
    {
        void Handle(TUseCaseResponse response);
    }
}
