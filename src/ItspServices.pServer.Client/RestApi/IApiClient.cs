using System.Threading.Tasks;
using ItspServices.pServer.Client.Datatypes;
using ItspServices.pServer.Client.Models;

namespace ItspServices.pServer.Client.RestApi
{
    interface IApiClient
    {
        Task<FolderModel> RequestFolderById(int? id);
        Task<DataModel> RequestDataByPath(string path);
        Task<int> SendCreateData(string path, DataModel dataModel);
        Task SendUpdateData(string path, DataModel dataModel);
        Task<KeyPair[]> RequestKeyPairsByFilePath(string path);
        Task SendCreateKeyPairWithFileId(int fileId, KeyPair keyPair);
    }
}
