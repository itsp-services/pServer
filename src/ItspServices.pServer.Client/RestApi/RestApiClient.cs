using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using ItspServices.pServer.Client.Datatypes;
using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.RestApi
{
    class RestApiClient : IApiClient
    {
        private IHttpClientFactory _provider;

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

        public async Task<ProtectedData> RequestDataByPath(string path)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/data/{path}"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return null;
                    return (await JsonSerializer.DeserializeAsync<DataModel>(await response.Content.ReadAsStreamAsync())).ToProtectedData();
                }
            }
        }

        public async Task<int> SendCreateData(string path, ProtectedData protectedData)
        {
            DataModel dataModel = protectedData.ToDataModel();
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

        public async Task SendUpdateData(string path, ProtectedData protectedData)
        {
            DataModel dataModel = protectedData.ToDataModel();
            using (HttpClient client = _provider.CreateClient())
            {
                string serializedModel = JsonSerializer.Serialize(dataModel);
                HttpContent content = new StringContent(serializedModel);
                using (HttpResponseMessage response = await client.PutAsync($"/api/protecteddata/data/{path}", content))
                {
                }
            }
        }

        public async Task<KeyPair[]> RequestKeyPairsByFilePath(string path)
        {
            using (HttpClient client = _provider.CreateClient())
            {
                using (HttpResponseMessage response = await client.GetAsync($"/api/protecteddata/key/{path}"))
                {
                    KeyPairModel[] keyPairModels = await JsonSerializer.DeserializeAsync<KeyPairModel[]>(await response.Content.ReadAsStreamAsync());
                    KeyPair[] keyPairs = new KeyPair[keyPairModels.Length];
                    for (int i = 0; i < keyPairModels.Length; i++)
                    {
                        keyPairs[i] = keyPairModels[i].ToKeyPair();
                    }
                    return keyPairs;
                }
            }
        }

        public async Task SendCreateKeyPairWithFileId(int fileId, KeyPair keyPair)
        {
            KeyPairModel keyPairModel = keyPair.ToKeyPairModel();
            using (HttpClient client = _provider.CreateClient())
            {
                string serializedModel = JsonSerializer.Serialize(keyPairModel);
                HttpContent content = new StringContent(serializedModel);
                using (HttpResponseMessage response = await client.PostAsync($"/api/protecteddata/key/{fileId}", content)) { }
            }
        }
    }
}
