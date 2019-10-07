using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Models;

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
                using (HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/folder/{id}"))
                    return await JsonSerializer.DeserializeAsync<FolderModel>(await response.Content.ReadAsStreamAsync());
            }
        }
        public async Task<DataModel> RequestProtectedDataById(int id)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/data/{id}"))
                    return await JsonSerializer.DeserializeAsync<DataModel>(await response.Content.ReadAsStreamAsync());
            }
        }

        public async Task SendUpdateData(int id, DataModel dataModel)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                string serializedModel = JsonSerializer.Serialize(dataModel);
                HttpContent content = new StringContent(serializedModel);
                using (HttpResponseMessage response = await client.PutAsync($"/api/protecteddata/data/{id}", content)) 
                {
                }
            }
        }

    }
}
