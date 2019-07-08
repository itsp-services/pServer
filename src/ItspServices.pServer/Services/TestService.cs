using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItspServices.pServer.Services
{
    public class TestService : ITestService
    {
        public DateTime GetDateTime()
            => DateTime.Now;
    }
}
