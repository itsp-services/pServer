using System;
using ItspServices.pServer.Client.Datatypes;

namespace ItspServices.pServer.Client.Models
{
    class DataModel
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }

    static class DataModelExtensions
    {
        public static DataModel ToDataModel(this ProtectedData protectedData)
        {
            return new DataModel()
            {
                Name = protectedData.Name,
                Data = Convert.ToBase64String(protectedData.Data)
            };
        }

        public static ProtectedData ToProtectedData(this DataModel dataModel)
        {
            ProtectedData protectedData = new ProtectedData()
            {
                Name = dataModel.Name,
                Data = Convert.FromBase64String(dataModel.Data)
            };
            return protectedData;
        }
    }
}
