using System.Net.Http;

namespace ItspServices.pServer.Client.Communicator
{
    public interface IClientProvider
    {
        HttpClient GetClient();
    }
}
