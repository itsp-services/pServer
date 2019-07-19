using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Abstraction.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProtectedData> DataRegister { get; set; } = new List<ProtectedData>();
        public List<Folder> Subfolder { get; set; } = new List<Folder>();
    }
}
