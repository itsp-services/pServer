using System;
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
            FolderModel folder = await _restClient.RequestFolderById(null);
            if (folder is null)
                throw new InvalidOperationException();

            folder = await FindFolder(1, folder);
            async Task<FolderModel> FindFolder(int position, FolderModel currentFolder)
            {
                int folderNameEnd = destination.IndexOf('/', position);
                if (folderNameEnd == -1)
                    return currentFolder;

                int length = folderNameEnd - position;
                foreach (int subFolderId in currentFolder.SubfolderIds)
                {
                    FolderModel subFolder = await _restClient.RequestFolderById(subFolderId);
                    if (string.Compare(destination, position, subFolder.Name, 0, length, StringComparison.InvariantCultureIgnoreCase) == 0)
                        return await FindFolder(folderNameEnd + 1, subFolder);
                }
                return null;
            }

            // TODO: request secrets
        }
    }
}
