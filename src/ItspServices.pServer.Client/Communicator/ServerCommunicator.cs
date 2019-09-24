using System;
using System.Collections.Generic;
using ItspServices.pServer.Client.Model;

namespace ItspServices.pServer.Client.Communicator
{
    public class ServerCommunicator
    {
        private IClientProvider _provider;

        public ServerCommunicator(IClientProvider provider)
        {
            _provider = provider;
        }

        public FolderModel RequestFolderById(int? id)
        {
            return new FolderModel()
            {
                ParentId = null,
                Name = "root",
                ProtectedDataIds = new List<int>(),
                SubfolderIds = new List<int>()
            };
        }
    }
}
