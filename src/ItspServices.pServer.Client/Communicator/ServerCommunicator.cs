using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Model;

namespace ItspServices.pServer.Client.Communicator
{
    public class ServerCommunicator
    {
        private IClientProvider _provider;

        public ServerCommunicator(IClientProvider provider)
        {
            _provider = provider;
        }

        public async Task<FolderModel> RequestFolderById(int? id)
        {
            HttpClient client = _provider.GetClient();
            string url = "/api/protecteddata/folder";

            if(id != null)
            {
                url += "/" + id;
            }
            HttpResponseMessage response = await client.GetAsync(url);
            FolderModel responseFolder = JsonSerializer.Deserialize<FolderModel>(await response.Content.ReadAsStringAsync());
            return responseFolder;
        }
    }
}
