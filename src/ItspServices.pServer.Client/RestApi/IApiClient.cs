using System.Threading.Tasks;
using ItspServices.pServer.Client.Datatypes;
using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.RestApi
{
    interface IApiClient
    {
        Task<FolderModel> RequestFolderById(int? id);
        Task<ProtectedData> RequestDataByPath(string path);
        Task<int> SendCreateData(string path, ProtectedData protectedData);
        Task SendUpdateData(string path, ProtectedData protectedData);
        Task<KeyPair[]> RequestKeyPairsByFilePath(string path);
        Task SendCreateKeyPairWithFileId(int fileId, KeyPair keyPair);
    }
}
