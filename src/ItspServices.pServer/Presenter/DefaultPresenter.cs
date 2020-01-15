using ItspServices.pServer.Abstraction.Models.UseCase.Response;
using ItspServices.pServer.Abstraction.UseCase;

namespace ItspServices.pServer.Presenter
{
    public class DefaultPresenter : IOutputPort<UseCaseResponse>
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public void Handle(UseCaseResponse response)
        {
            Success = response.Success;
            Message = response.Message;
        }
    }
}
