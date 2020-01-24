using System.Text;
using System.Threading.Tasks;
using ItspServices.pServer.Client.Datatypes;
using ItspServices.pServer.Client.RestApi;
using ItspServices.pServer.Client.Security;
using ItspServices.pServer.Client.Security.Keys;

namespace ItspServices.pServer.Client
{
    public class ProtectedDataClient
    {
        private IApiClient _apiClient = null;
        private IDataEncryptor _dataEncryptor = null;
        private ILocalKeysController _localKeysController;
        private IKeyFactory _keyFactory = null;

        public ProtectedDataClient(ILocalKeysController localKeysController)
        {
            _localKeysController = localKeysController;
        }

        public async Task Set(string destination, string data)
        {
            
            Key publicKey = new Key(_localKeysController.GetPublicKey());
            ProtectedData protectedData = await _apiClient.RequestDataByPath(destination);
            if (protectedData == null)
            {
                await PostNewData(destination, data, publicKey);
            }
            else
            {
                await UpdateExistingData(destination, data, publicKey, protectedData);
            }
        }

        private async Task UpdateExistingData(string destination, string data, Key publicKey, ProtectedData protectedData)
        {
            Key privateKey = new Key(_localKeysController.GetPrivateKey());
            Key symmetricKey = null;
            KeyPair[] keyPairs = await _apiClient.RequestKeyPairsByFilePath(destination);
            foreach (KeyPair keyPair in keyPairs)
            {
                if (keyPair.PublicKey.GetBase64() == publicKey.GetBase64())
                {
                    symmetricKey = _dataEncryptor.AsymmetricDecryptData(new Key(keyPair.SymmetricKey), privateKey);
                    break;
                }
            }
            // TODO: case no symKey
            protectedData.Data = _dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(data), symmetricKey);
            await _apiClient.SendUpdateData(destination, protectedData);
        }

        private async Task PostNewData(string destination, string data, Key publicKey)
        {
            Key symmetricKey = _keyFactory.CreateSymmetricKey();
            int fileId = await _apiClient.SendCreateData(destination, new ProtectedData
            {
                Name = destination.Substring(destination.LastIndexOf('/') + 1),
                Data = _dataEncryptor.SymmetricEncryptData(Encoding.Default.GetBytes(data), symmetricKey)
            });
            await _apiClient.SendCreateKeyPairWithFileId(fileId, new KeyPair
            {
                PublicKey = publicKey,
                SymmetricKey = _dataEncryptor.AsymmetricEncryptData(symmetricKey, publicKey)
            });
        }

        internal void SetClient(IApiClient client)
        {
            _apiClient = client;
        }

        internal void SetEncryptor(IDataEncryptor encryptor)
        {
            _dataEncryptor = encryptor;
        }

        internal void SetKeyFactory(IKeyFactory keyFactory)
        {
            _keyFactory = keyFactory;
        }
    }
}
