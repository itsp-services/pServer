using System;
using System.Net.Http;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Models;
using ItspServices.pServer.Client.RestApi;
using ItspServices.pServer.Client.Security;

namespace ItspServices.pServer.Client
{
    public class ProtectedDataClient
    {
        private RestApiClient _restClient;
        private ILocalKeysController _localKeysController;
        private IDataEncryptor _dataEncryptor;

        public ProtectedDataClient(IHttpClientFactory factory, ILocalKeysController localKeysController, IDataEncryptor encryptor)
        {
            _restClient = new RestApiClient(factory);
            _localKeysController = localKeysController;
            _dataEncryptor = encryptor;
        }

        public async Task Set(string destination, string data)
        {
            string publicKey = _localKeysController.GetPublicKey();
            DataModel dataModel = await _restClient.RequestDataByPath(destination);
            if (dataModel == null)
            {
                string symmetricKey = _localKeysController.CreateSymmetricKey();
                int fileId = await _restClient.SendCreateData(destination, new DataModel
                {
                    Name = destination.Substring(destination.LastIndexOf('/') + 1),
                    Data = _dataEncryptor.EncryptData(data, symmetricKey)
                });
                await _restClient.SendCreateKeyPairWithFileId(fileId, new KeyPairModel
                {
                    PublicKey = publicKey,
                    SymmetricKey = _dataEncryptor.EncryptData(symmetricKey, publicKey)
                });
            }
            else
            {
                string privateKey = _localKeysController.GetPrivateKey();
                string symmetricKey = null;
                KeyPairModel[] keyPairModels = await _restClient.RequestKeyPairsByFilePath(destination);
                foreach (KeyPairModel keyPairModel in keyPairModels)
                {
                    if (keyPairModel.PublicKey == publicKey)
                    {
                        symmetricKey = _dataEncryptor.DecryptData(keyPairModel.SymmetricKey, privateKey);
                        break;
                    }
                }
                // TODO: case no symKey
                dataModel.Data = _dataEncryptor.EncryptData(data, symmetricKey);
                await _restClient.SendUpdateData(destination, dataModel);
            }
        }
    }
}
