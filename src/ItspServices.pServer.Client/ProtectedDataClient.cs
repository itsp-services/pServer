using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Models;
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

        public async Task Set(string destination, byte[] protectedData)
        {
            FolderModel folder = await _restClient.RequestFolderById(null);
            if (folder is null)
                throw new InvalidOperationException();

            folder = await FindFolder(destination, folder);

            int? dataId = null;
            DataModel dataModel = null;
            foreach (int protectedDataId in folder.ProtectedDataIds)
            {
                dataModel = await _restClient.RequestProtectedDataById(protectedDataId);
                if (destination.EndsWith(dataModel.Name))
                {
                    dataId = protectedDataId;
                    break;
                }
            }
            if (dataId != null)
            {
                // TODO: Encrypt data with symmetric of requested datamodel
                dataModel.Data = protectedData;
                await _restClient.SendUpdateData((int) dataId, dataModel);
            } 
            else
            {
                // TODO: Create new datamodel and encrypt data with new symmetric key and send create request


            }
        }

        private async Task<FolderModel> FindFolder(string destination, FolderModel currentFolder, string path = "")
        {
            if (destination.IndexOf('/', path.Length + 1) == -1)
                return currentFolder;

            FolderModel subFolder = null;
            foreach (int subFolderId in currentFolder.SubfolderIds)
            {
                subFolder = await _restClient.RequestFolderById(subFolderId);
                if (destination.StartsWith(path + '/' + subFolder.Name))
                    return await FindFolder(destination, subFolder, path += '/' + subFolder.Name);
            }
            return null;
        }
    }
}
