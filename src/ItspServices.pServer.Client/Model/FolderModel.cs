using System.Collections.Generic;

namespace ItspServices.pServer.Client.Model
{
    class FolderModel
    {
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> ProtectedDataIds { get; set; }
        public IEnumerable<int> SubfolderIds { get; set; }
    }
}
