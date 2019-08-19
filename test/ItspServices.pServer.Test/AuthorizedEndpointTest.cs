using ItspServices.pServer.Abstraction;
using ItspServices.pServer.Abstraction.Models;
using ItspServices.pServer.Abstraction.Repository;
using ItspServices.pServer.Abstraction.Units;
using ItspServices.pServer.Models;
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
    public class AuthorizedEndpointTest
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
        public static ProtectedData DataWithoutRegisteredUsers;

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
            Admin.PublicKeys.Add(new Key() { Id = 1, KeyData = Encoding.UTF8.GetBytes("EALAAgAEMASQBGcgDzAG4AaQBjAG") });

            User = new User()
            {
                Id = 1,
                UserName = "User",
                NormalizedUserName = "USER",
                Role = "User"
            };
            User.PasswordHash = new PasswordHasher<User>().HashPassword(User, UserPassword);
            User.PublicKeys.Add(new Key() { Id = 0, KeyData = Encoding.UTF8.GetBytes("lHPrzg5XPAOBOp0KoVdDaaxXbXmQ") });
            User.PublicKeys.Add(new Key() { Id = 1, KeyData = Encoding.UTF8.GetBytes("KoVdDaaxXbXmQlHPrzg5XPAOBOp0") });

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
            entry1.EncryptedKeys.Add(new SymmetricKey() { MatchingPublicKeyId = 1, KeyData = Encoding.UTF8.GetBytes("VdD0Koaag5XPAOBOpxXbXmQlHPrz") });
            entry1.EncryptedKeys.Add(new SymmetricKey() { MatchingPublicKeyId = 0, KeyData = Encoding.UTF8.GetBytes("0KoVdDaaxXbXmQlHPrzg5XPAOBOp") });
            Data0.Users.RegisterEntries.Add(entry1);

            Data1 = new ProtectedData()
            {
                Id = 1,
                Name = "Data1",
                OwnerId = 1
            };
            Data1.Data = Encoding.UTF8.GetBytes("Data1");
            var entry2 = new UserRegisterEntry()
            {
                User = Admin,
                Permission = Permission.VIEW
            };
            entry2.EncryptedKeys.Add(new SymmetricKey() { MatchingPublicKeyId = 1, KeyData = Encoding.UTF8.GetBytes("VdD0Koaag5XPAOBOpxXbXmQlHPrz") });
            entry2.EncryptedKeys.Add(new SymmetricKey() { MatchingPublicKeyId = 0, KeyData = Encoding.UTF8.GetBytes("0KoVdDaaxXbXmQlHPrzg5XPAOBOp") });
            Data1.Users.RegisterEntries.Add(entry2);

            DataWithoutRegisteredUsers = new ProtectedData()
            {
                Id = 2,
                Name = "Data2",
                OwnerId = 999
            };
            DataWithoutRegisteredUsers.Data = Encoding.UTF8.GetBytes("Data2");

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

            using (HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/folder"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string content = await response.Content.ReadAsStringAsync();

                JToken expected = JToken.FromObject(new {
                    parentId = default(int?),
                    name = "root",
                    protectedDataIds = default(int[]),
                    subfolderIds = new[] { 1 }
                });

                Assert.IsTrue(JToken.DeepEquals(JToken.Parse(content), expected));
            }
        }

        [TestMethod]
        public async Task ProtectedDataGetFolderById()
        {
            ProtectedDataRepository.Setup(x => x.GetFolderById(1)).Returns(Folder1);

            using (HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/folder/1"))
            { 
                string content = await response.Content.ReadAsStringAsync();

                JToken expected = JToken.FromObject(new
                {
                    parentId = default(int?),
                    name = "Folder1",
                    protectedDataIds = default(int[]),
                    subfolderIds = default(int[])
                });

                Assert.IsTrue(JToken.DeepEquals(JToken.Parse(content), expected));
            }
        }

        [TestMethod]
        public async Task ProtectedDataGetById()
        {
            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(Data0);

            var response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            DataModel data = JsonConvert.DeserializeObject<DataModel>(content);

            Assert.AreEqual(Data0.Name, data.Name);
            CollectionAssert.AreEquivalent(Data0.Data, data.Data);
            CollectionAssert.AreEquivalent(User.PublicKeys[0].KeyData, data.KeyPairs.ToList()[0].PublicKey);
            CollectionAssert.AreEquivalent(Data0.Users.RegisterEntries[0].EncryptedKeys[0].KeyData, data.KeyPairs.ToList()[0].SymmetricKey);
            CollectionAssert.AreEquivalent(Data0.Users.RegisterEntries[0].EncryptedKeys[1].KeyData, data.KeyPairs.ToList()[1].SymmetricKey);
        }

        [TestMethod]
        public async Task UnauthorizedRequestData()
        {
            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(DataWithoutRegisteredUsers);
            var response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task NotFoundRequestData()
        {
            var response = await UserClient.GetAsync("/api/protecteddata/data/999");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task RequestWithViewPermission()
        {
            ProtectedDataRepository.Setup(x => x.GetById(1)).Returns(Data1);

            // Admin should have view permission for protected data 1
            var response = await AdminClient.GetAsync("/api/protecteddata/data/1");
            string output = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task AddProtectedDataToRootFolder()
        {
            DataModel model = new DataModel()
            {
                Name = "NewData",
                Data = Encoding.UTF8.GetBytes("Data"),
                KeyPairs = new[] {
                    new KeyPairModel() {
                        PublicKey = User.PublicKeys[0].KeyData,
                        SymmetricKey = Encoding.UTF8.GetBytes("mQlHPrzg5XPAOBOp0KoVdDaaxXbX")
                    }
                }
            };

            Mock<IUnitOfWork<ProtectedData>> unit = new Mock<IUnitOfWork<ProtectedData>>();
            unit.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetFolderById(null)).Returns(RootFolder);
            ProtectedDataRepository.Setup(x => x.AddToFolder(It.IsAny<ProtectedData>(), RootFolder)).Returns(unit.Object).Verifiable();

            var response = await UserClient.PostAsJsonAsync("/api/protecteddata/data", model);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            ProtectedDataRepository.Verify(x => x.AddToFolder(It.IsAny<ProtectedData>(), RootFolder));
            unit.Verify(x => x.Complete());
        }

        [TestMethod]
        public async Task AddProtectedDataToExplicitFolder()
        {
            DataModel model = new DataModel()
            {
                Name = "NewData",
                Data = Encoding.UTF8.GetBytes("Data"),
                KeyPairs = new[] {
                    new KeyPairModel() {
                        PublicKey = User.PublicKeys[0].KeyData,
                        SymmetricKey = Encoding.UTF8.GetBytes("mQlHPrzg5XPAOBOp0KoVdDaaxXbX")
                    }
                }
            };

            Mock<IUnitOfWork<ProtectedData>> unit = new Mock<IUnitOfWork<ProtectedData>>();
            unit.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.AddToFolder(It.IsAny<ProtectedData>(), Folder1)).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetFolderById(1)).Returns(Folder1);
            ProtectedDataRepository.Setup(x => x.AddToFolder(It.IsAny<ProtectedData>(), It.IsAny<Folder>())).Returns(unit.Object);

            var response = await UserClient.PostAsJsonAsync("/api/protecteddata/data/1", model);

            ProtectedDataRepository.Verify(x => x.AddToFolder(It.IsAny<ProtectedData>(), Folder1));
            unit.Verify(x => x.Complete());
        }

        [TestMethod]
        public async Task UpdateProtectedData_UserHasWritePermissionShouldSucceed()
        {
            ProtectedData data = new ProtectedData()
            {
                Id = 0,
                OwnerId = 999,
                Name = "NewData",
                Data = Encoding.UTF8.GetBytes("OldData")
            };
            var entry = new UserRegisterEntry() {
                User = User,
                Permission = Permission.WRITE // User now has write permission and should be able to update
            };
            entry.EncryptedKeys.Add(new SymmetricKey() { MatchingPublicKeyId = 0, KeyData = Encoding.UTF8.GetBytes("mQlHPrzg5XPAOBOp0KoVdDaaxXbX") });
            data.Users.RegisterEntries.Add(entry);

            Mock <IUnitOfWork<ProtectedData>> unit = new Mock<IUnitOfWork<ProtectedData>>();
            unit.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(data);
            ProtectedDataRepository.Setup(x => x.Update(It.IsAny<ProtectedData>())).Returns(unit.Object).Verifiable();

            var response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();

            dynamic requestedData = JToken.Parse(content);
            requestedData.Data = Encoding.UTF8.GetBytes("NewData");

            response = await UserClient.PostAsJsonAsync("/api/protecteddata/data/update/0", (JToken) requestedData);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            ProtectedDataRepository.Verify(x => x.Update(It.IsAny<ProtectedData>()));
            unit.Verify(x => x.Complete());

            Assert.AreEqual("NewData", Encoding.UTF8.GetString(data.Data));
        }

        [TestMethod]
        public async Task UpdateProtectedData_UserHasReadPermissionShouldFail()
        {
            ProtectedData data = new ProtectedData()
            {
                Id = 0,
                OwnerId = 999,  // User is definitely not the owner of this data
                Name = "NewData",
                Data = Encoding.UTF8.GetBytes("OldData")
            };
            var entry = new UserRegisterEntry()
            {
                User = User,
                Permission = Permission.READ // User only has read permission and should not be able to update
            };
            entry.EncryptedKeys.Add(new SymmetricKey() { MatchingPublicKeyId = 0, KeyData = Encoding.UTF8.GetBytes("mQlHPrzg5XPAOBOp0KoVdDaaxXbX") });
            data.Users.RegisterEntries.Add(entry);

            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(data);

            var response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();

            dynamic requestedData = JToken.Parse(content);
            requestedData.Data = Encoding.UTF8.GetBytes("NewData");

            response = await UserClient.PostAsJsonAsync("/api/protecteddata/data/update/0", (JToken)requestedData);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task RemoveProtectedData_UserIsOwnerShouldSucceed()
        {
            Mock<IUnitOfWork<ProtectedData>> uow = new Mock<IUnitOfWork<ProtectedData>>();
            uow.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetById(1)).Returns(Data1);
            ProtectedDataRepository.Setup(x => x.Remove(Data1)).Returns(uow.Object).Verifiable();

            var response = await UserClient.DeleteAsync("/api/protecteddata/data/1");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            ProtectedDataRepository.Verify(x => x.Remove(Data1));
            uow.Verify(x => x.Complete());
        }

        [TestMethod]
        public async Task RemoveProtectedData_UserHasReadPermissionShouldFail()
        {
            Mock<IUnitOfWork<ProtectedData>> uow = new Mock<IUnitOfWork<ProtectedData>>();
            uow.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(Data0);
            ProtectedDataRepository.Setup(x => x.Remove(Data0)).Returns(uow.Object).Verifiable();

            var response = await UserClient.DeleteAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            uow.VerifyNoOtherCalls();
        }
    }
}