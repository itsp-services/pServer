using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Model;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.ClientTest")]

namespace ItspServices.pServer.Client.RestApi
{
    class RestApiClient
    {
        IHttpClientFactory _provider;

        public RestApiClient(IHttpClientFactory provider)
        {
            _provider = provider;
        }

        public async Task<FolderModel> RequestFolderById(int? id)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                using(HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/folder/{id}"))
                    return await JsonSerializer.DeserializeAsync<FolderModel>(await response.Content.ReadAsStreamAsync());
            }
        }
    }
}
