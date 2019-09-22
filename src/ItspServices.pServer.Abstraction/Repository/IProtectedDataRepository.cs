using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Repository
{
    public interface IProtectedDataRepository : IRepository<ProtectedData, int>
    {
        IUnitOfWork<ProtectedData> AddToFolder(ProtectedData data, Folder folder);
        Folder GetFolderById(int? folderId);
    }
}
