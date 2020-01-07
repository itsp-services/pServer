using System;
using System.Text;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Models;
using ItspServices.pServer.Client.RestApi;
using ItspServices.pServer.Client.Security;

namespace ItspServices.pServer.Client
{
    public class ProtectedDataClient
    {
        private IApiClient _apiClient = null;
        private IDataEncryptor _dataEncryptor = null;
        private ILocalKeysController _localKeysController;

        public ProtectedDataClient(ILocalKeysController localKeysController)
        {
            _localKeysController = localKeysController;
        }

        public async Task Set(string destination, string data)
        {
            string publicKey = _localKeysController.GetPublicKey();
            DataModel dataModel = await _apiClient.RequestDataByPath(destination);
            if (dataModel == null)
            {
                string symmetricKey = Convert.ToBase64String(_dataEncryptor.CreateSymmetricKey());
                int fileId = await _apiClient.SendCreateData(destination, new DataModel
                {
                    Name = destination.Substring(destination.LastIndexOf('/') + 1),
                    Data = Convert.ToBase64String(_dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(data), Convert.FromBase64String(symmetricKey)))
                });
                await _apiClient.SendCreateKeyPairWithFileId(fileId, new KeyPairModel
                {
                    PublicKey = publicKey,
                    SymmetricKey = Convert.ToBase64String(_dataEncryptor.AsymmetricEncryptData(Convert.FromBase64String(symmetricKey), Convert.FromBase64String(publicKey)))
                });
            }
            else
            {
                string privateKey = _localKeysController.GetPrivateKey();
                string symmetricKey = null;
                KeyPairModel[] keyPairModels = await _apiClient.RequestKeyPairsByFilePath(destination);
                foreach (KeyPairModel keyPairModel in keyPairModels)
                {
                    if (keyPairModel.PublicKey == publicKey)
                    {
                        symmetricKey = Convert.ToBase64String(_dataEncryptor.AsymmetricDecryptData(Convert.FromBase64String(keyPairModel.SymmetricKey), Convert.FromBase64String(privateKey)));
                        break;
                    }
                }
                // TODO: case no symKey
                dataModel.Data = Convert.ToBase64String(_dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(data), Convert.FromBase64String(symmetricKey)));
                await _apiClient.SendUpdateData(destination, dataModel);
            }
        }

        internal void SetClient(IApiClient client)
        {
            _apiClient = client;
        }

        internal void SetEncryptor(IDataEncryptor encryptor)
        {
            _dataEncryptor = encryptor;
        }
    }
}
