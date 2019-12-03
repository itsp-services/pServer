using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.RestApi
{
    class RestApiClient : IApiClient
    {
        public IHttpClientFactory _provider { get; set; }

        public async Task<FolderModel> RequestFolderById(int? id)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/folder/{id}"))
                    return await JsonSerializer.DeserializeAsync<FolderModel>(await response.Content.ReadAsStreamAsync());
            }
        }

        public async Task<DataModel> RequestDataByPath(string path)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/data/{path}"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return null;
                    return await JsonSerializer.DeserializeAsync<DataModel>(await response.Content.ReadAsStreamAsync());
                }
            }
        }

        public async Task<int> SendCreateData(string path, DataModel dataModel)
        {
            DataModelWithPath dataModelWithPath = new DataModelWithPath
            {
                DataModel = dataModel,
                Path = path
            };
            using (HttpClient client = _provider.CreateClient())
            {
                string serializedModel = JsonSerializer.Serialize(dataModelWithPath);
                HttpContent content = new StringContent(serializedModel);
                using (HttpResponseMessage response = await client.PostAsync($"/api/protecteddata/data/", content))
                {
                    return Int32.Parse(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task SendUpdateData(string path, DataModel dataModel)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                string serializedModel = JsonSerializer.Serialize(dataModel);
                HttpContent content = new StringContent(serializedModel);
                using (HttpResponseMessage response = await client.PutAsync($"/api/protecteddata/data/{path}", content))
                {
                }
            }
        }

        public async Task<KeyPairModel[]> RequestKeyPairsByFilePath(string path)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/key/{path}"))
                {
                    return await JsonSerializer.DeserializeAsync<KeyPairModel[]>(await response.Content.ReadAsStreamAsync());
                }
            }
        }

        public async Task SendCreateKeyPairWithFileId(int fileId, KeyPairModel keyPairModel)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                string serializedModel = JsonSerializer.Serialize(keyPairModel);
                HttpContent content = new StringContent(serializedModel);
                using (HttpResponseMessage response = await client.PostAsync($"/api/protecteddata/key/{fileId}", content))
                {
                }
            }
        }
    }
}
