using System;
using System.Collections.Generic;
using System.Text;

namespace ItspServices.pServer.Test.Mock.ServerResponse
{
    class ServerResponse
    {
        public static IEnumerable<object> JsonResponseTest = new[] {
            new { Name = "Test", Value = 3 },
            new { Name = "Test2", Value = 4 },
            new { Name = "Test3", Value = 5 }
        };
    }
}
