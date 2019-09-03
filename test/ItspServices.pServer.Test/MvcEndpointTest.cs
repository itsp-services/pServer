using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ItspServices.pServer.Test
{
    [TestClass]
    public class MvcEndpointTest
    {
        public static Mock<IRepositoryManager> RepositoryManager = new Mock<IRepositoryManager>();

        public Mock<IUserRepository> UserRepository;
        public HttpClient Client { get; set; }

        class WebApplicationFactory : WebApplicationFactory<Startup>
        {
            protected override IWebHostBuilder CreateWebHostBuilder() =>
                Program.CreateWebHostBuilder(new string[0]);

            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                base.ConfigureWebHost(builder);
                builder.ConfigureTestServices(services =>
                {
                    foreach (ServiceDescriptor serviceDescriptor in services.Where(x => x.ServiceType == typeof(IRepositoryManager)).ToList())
                        services.Remove(serviceDescriptor);

                    services.AddSingleton(typeof(IRepositoryManager), RepositoryManager.Object);
                });
            }
        }

        [TestInitialize]
        public void Init()
        {
            UserRepository = new Mock<IUserRepository>();
            RepositoryManager.Setup(x => x.UserRepository).Returns(UserRepository.Object);

            Client = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
        }

        [TestMethod]
        public async Task GetAccountControllerLoginTest()
        {
            Assert.AreEqual(HttpStatusCode.OK, (await Client.GetAsync("/Account/Login")).StatusCode);
        }

        [TestMethod]
        public async Task ReturnUrlFormTest()
        {
            HttpResponseMessage response = await Client.GetAsync("/Account/Login?returnurl=%2F");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string expectedToken = "action=\"/Account/Login?returnurl=%2F\"";
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains(expectedToken), "Return Url not found: " + expectedToken);
        }

        [TestMethod]
        public async Task PostCredentialsLoginTest()
        {
            User user = new User();
            user.UserName = "Foo";
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Bar123456789");
            UserRepository.Setup(x => x.GetUserByNormalizedName("FOO")).Returns(user);

            HttpResponseMessage response = await Client.PostAsync("/Account/Login?returnurl=%2F", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "Foo"),
                new KeyValuePair<string, string>("Password", "Bar123456789")
            }));

            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<title>Home Page"), "Redirect failed.");
        }

        [TestMethod]
        public async Task GetAccountControllerRegisterTest()
        {
            Assert.AreEqual(HttpStatusCode.OK, (await Client.GetAsync("/Account/Register")).StatusCode);
        }

        [TestMethod]
        public async Task PostCredentialsRegisterTest()
        {
            User user = new User();
            UserRepository.Setup(x => x.GetUserByNormalizedName("FOO")).Returns(() => user.NormalizedUserName == "FOO" ? user : null);

            Mock<IAddUnitOfWork<User>> unit = new Mock<IAddUnitOfWork<User>>();
            unit.Setup(x => x.Entity).Returns(user);
            unit.Setup(x => x.Complete()).Verifiable();

            UserRepository.Setup(x => x.Add()).Returns(unit.Object).Verifiable();
            UserRepository.Setup(x => x.GetUserByNormalizedName("Foo"));

            HttpResponseMessage response = await Client.PostAsync("/Account/Register?returnurl=%2F", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", "Foo"),
                new KeyValuePair<string, string>("Password", "Bar123456789!"),
                new KeyValuePair<string, string>("ConfirmPassword", "Bar123456789!")
            }));

            unit.Verify(x => x.Complete());
            UserRepository.Verify(x => x.Add());
            string content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("<title>Home Page"), "Redirect failed.");
        }
    }
}
