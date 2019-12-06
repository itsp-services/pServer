using System.Threading.Tasks;
using ItspServices.pServer.Client.Models;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ItspServices.pServer.ClientTest")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace ItspServices.pServer.Client.RestApi
{
    interface IApiClient
    {
        Task<FolderModel> RequestFolderById(int? id);
        Task<DataModel> RequestDataByPath(string path);
        Task<int> SendCreateData(string path, DataModel dataModel);
        Task SendUpdateData(string path, DataModel dataModel);
        Task<KeyPairModel[]> RequestKeyPairsByFilePath(string path);
        Task SendCreateKeyPairWithFileId(int fileId, KeyPairModel keyPairModel);
    }
}
