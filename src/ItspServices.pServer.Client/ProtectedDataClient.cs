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
            // FolderModel folder = await FindFolder(root, folderPath);

            // TODO: request secrets
        }

        private async Task<FolderModel> FindFolder(FolderModel currentFolder, string destination, string path = "")
        {
            if (currentFolder.Name != "root")
                path += '/' + currentFolder.Name;
            if (path == destination)
                return currentFolder;
            if (!destination.StartsWith(path))
                return null;

            FolderModel folder = null;
            foreach (int folderId in currentFolder.SubfolderIds)
            {
                folder = await FindFolder(currentFolder, destination, path);
                if (folder != null)
                    break;
            }
            return folder;
        }
    }
}
