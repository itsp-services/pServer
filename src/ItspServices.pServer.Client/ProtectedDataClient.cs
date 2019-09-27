using System.Net.Http;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Model;
using ItspServices.pServer.Client.RestApi;

namespace ItspServices.pServer.Client
{
    public class ProtectedDataClient
    {
        private RestApiClient _restClient;

        public ProtectedDataClient(IHttpClientFactory factory)
        {
            _restClient = new RestApiClient(factory);
        }

        public async Task Set(string destination, string protectedData)
        {
            FolderModel root = await _restClient.RequestFolderById(null);
            string folderPath = destination.Substring(0, destination.LastIndexOf('/'));
            FolderModel folder = await FindFolder("", folderPath, root);

            // TODO: request secrets
        }

        private async Task<FolderModel> FindFolder(string path, string destination, FolderModel currentFolder)
        {
            if (currentFolder.Name != "root")
                path += '/' + currentFolder.Name;
            if (path == destination)
                return currentFolder;
            foreach (int folderId in currentFolder.SubfolderIds)
            {
                return await FindFolder(path, destination, await _restClient.RequestFolderById(folderId));
            }
            return null;
        }
    }
}
