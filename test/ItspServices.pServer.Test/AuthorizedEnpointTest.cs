using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ItspServices.pServer.Test
{

    [TestClass]
    public class AuthorizedEnpointTest
    {
        public static HttpClient AdminClient;
        public static HttpClient UserClient;

        public static User Admin;
        public static User User;

        public static string AdminPassword = "Admin12345!";
        public static string UserPassword = "User12345!";

        public static Folder RootFolder;
        public static Folder Folder1;

        public static ProtectedData Data0;
        public static ProtectedData Data1;

        public static Mock<IRepositoryManager> RepositoryManager = new Mock<IRepositoryManager>();
        public static Mock<IUserRepository> UserRepository = new Mock<IUserRepository>();
        public Mock<IProtectedDataRepository> ProtectedDataRepository;

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

        [ClassInitialize]
        public static async Task InitAsync(TestContext context)
        {
            Admin = new User()
            {
                Id = 0,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Role = "Admin"
            };
            Admin.PasswordHash = new PasswordHasher<User>().HashPassword(Admin, AdminPassword);
            Admin.PublicKeys.Add(new Key() { Id = 0, KeyData = Encoding.UTF8.GetBytes("cgDzAG4AaQBjAGEALAAgAEMASQBG") });

            User = new User()
            {
                Id = 1,
                UserName = "User",
                NormalizedUserName = "USER",
                Role = "User"
            };
            User.PasswordHash = new PasswordHasher<User>().HashPassword(User, UserPassword);
            User.PublicKeys.Add(new Key() { Id = 0, KeyData = Encoding.UTF8.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ") });

            UserRepository.Setup(x => x.GetUserByNormalizedName(Admin.NormalizedUserName)).Returns(Admin);
            UserRepository.Setup(x => x.GetUserByNormalizedName(User.NormalizedUserName)).Returns(User);
            UserRepository.Setup(x => x.GetById(Admin.Id)).Returns(Admin);
            UserRepository.Setup(x => x.GetById(User.Id)).Returns(User);

            Folder1 = new Folder()
            {
                Id = 1,
                Name = "Folder1"
            };

            RootFolder = new Folder()
            {
                Id = 0,
                Name = "root",
            };
            RootFolder.Subfolder = new List<Folder>();
            RootFolder.Subfolder.Add(Folder1);

            Data0 = new ProtectedData()
            {
                Id = 0,
                Name = "Data0",
                OwnerId = 0
            };
            Data0.Data = Encoding.UTF8.GetBytes("Data0");

            var entry1 = new UserRegisterEntry()
            {
                User = User,
                Permission = Permission.READ
            };
            entry1.EncryptedKeys.Add(new SymmetricKey() { MatchingPublicKeyId = 0, KeyData = Encoding.UTF8.GetBytes("0KoVdDaaxXbXmQlHPrzg5XPAOBOp") });
            Data0.Users.RegisterEntries.Add(entry1);

            Data1 = new ProtectedData()
            {
                Id = 1,
                Name = "Data1",
                OwnerId = 1
            };
            Data1.Data = Encoding.UTF8.GetBytes("data1");

            RepositoryManager.Setup(x => x.UserRepository).Returns(UserRepository.Object);

            AdminClient = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            UserClient = new WebApplicationFactory().CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            await AdminClient.PostAsync("/Account/Login", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", Admin.UserName),
                new KeyValuePair<string, string>("Password", AdminPassword)
            }));
            await UserClient.PostAsync("/Account/Login", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", User.UserName),
                new KeyValuePair<string, string>("Password", UserPassword)
            }));
        }

        [TestInitialize]
        public void TestInit()
        {
            ProtectedDataRepository = new Mock<IProtectedDataRepository>();
            RepositoryManager.Setup(x => x.ProtectedDataRepository).Returns(ProtectedDataRepository.Object);
        }

        [TestMethod]
        public async Task ProtectedDataGetRootFolder()
        {
            ProtectedDataRepository.Setup(x => x.GetFolderById(null)).Returns(RootFolder);

            var response = await UserClient.GetAsync("/api/protecteddata/folder");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            JToken token = JToken.Parse(content);
            Assert.AreEqual(token["id"].Value<int>(), 0);
            Assert.AreEqual(token["name"].Value<string>(), "root");
        }

        [TestMethod]
        public async Task ProtectedDataGetFolderById()
        {
            ProtectedDataRepository.Setup(x => x.GetFolderById(1)).Returns(Folder1);

            var response = await UserClient.GetAsync("/api/protecteddata/folder/1");
            string content = await response.Content.ReadAsStringAsync();
            JToken token = JToken.Parse(content);
            Assert.AreEqual(token["id"].Value<int>(), 1);
            Assert.AreEqual(token["name"].Value<string>(), "Folder1");
        }

        [TestMethod]
        public async Task ProtectedDataGetById()
        {
            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(Data0);

            var response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            ProtectedData data = JsonConvert.DeserializeObject<ProtectedData>(content);
        }

        [TestMethod]
        public async Task UnauthorizedRequestData()
        {
            ProtectedDataRepository.Setup(x => x.GetById(1)).Returns(Data1);

            var response = await AdminClient.GetAsync("/api/protecteddata/data/1");
            string output = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task NotFoundRequestData()
        {
            var response = await UserClient.GetAsync("/api/protecteddata/data/999");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}