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
        #region Fields
        static WebApplicationFactory ClientFactory;
        static HttpClient AdminClient;
        static HttpClient UserClient;

        static User Admin;
        static User User;

        const string AdminPassword = "Admin12345!";
        const string UserPassword = "User12345!";

        static readonly Mock<IRepositoryManager> RepositoryManager = new Mock<IRepositoryManager>();
        static readonly Mock<IUserRepository> UserRepository = new Mock<IUserRepository>();
        Mock<IProtectedDataRepository> ProtectedDataRepository;
        #endregion

        #region Nested classes
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
        #endregion

        #region Initialize and cleanup methods
        [ClassInitialize]
        public static async Task InitAsync(TestContext _)
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

            RepositoryManager.Setup(x => x.UserRepository).Returns(UserRepository.Object);

            ClientFactory = new WebApplicationFactory();
            AdminClient = ClientFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            UserClient = ClientFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            await AdminClient.PostAsync("/Account/Login", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", Admin.UserName),
                new KeyValuePair<string, string>("Password", AdminPassword)
            }));
            await UserClient.PostAsync("/Account/Login", new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("Username", User.UserName),
                new KeyValuePair<string, string>("Password", UserPassword)
            }));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            ClientFactory?.Dispose();
            AdminClient?.Dispose();
            UserClient?.Dispose();
            ClientFactory = null;
            AdminClient = null;
            UserClient = null;
        }

        [TestInitialize]
        public void TestInit()
        {
            ProtectedDataRepository = new Mock<IProtectedDataRepository>();
            RepositoryManager.Setup(x => x.ProtectedDataRepository).Returns(ProtectedDataRepository.Object);
        }
        #endregion

        #region Test methods
        [TestMethod]
        public async Task GetFolderWithoutProvidingAnId_ShouldReturnTheRootFolder()
        {
            Folder rootFolder = new Folder
            {
                Id = 0,
                Name = "root"
            };
            ProtectedDataRepository.Setup(x => x.GetFolderById(null)).Returns(rootFolder);

            using (HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/folder"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                string content = await response.Content.ReadAsStringAsync();

                JToken expected = JToken.FromObject(new {
                    parentId = default(int?),
                    name = "root",
                    protectedDataIds = default(int[]),
                    subfolderIds = default(int[])
                });

                Assert.IsTrue(JToken.DeepEquals(JToken.Parse(content), expected));
            }
        }

        [TestMethod]
        public async Task GetFolderWithProvidingAnId_ShouldReturnThatFolder()
        {
            Folder folder = new Folder()
            {
                Id = 1,
                Name = "Folder1"
            };

            ProtectedDataRepository.Setup(x => x.GetFolderById(1)).Returns(folder);

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
        public async Task GetFolderByIdThatDoesHaveSubfolders_ShouldReturnTheirIdsInTheSubfolderIdsMember()
        {
            Folder folder = new Folder()
            {
                Id = 1,
                Name = "Folder1",
                Subfolder = new[] {
                    new Folder
                    {
                        Id = 2,
                        Name = "SubFolder1"
                    }
                }.ToList()
            };

            ProtectedDataRepository.Setup(x => x.GetFolderById(1)).Returns(folder);

            using (HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/folder/1"))
            {
                string content = await response.Content.ReadAsStringAsync();

                JToken expected = JToken.FromObject(new
                {
                    parentId = default(int?),
                    name = "Folder1",
                    protectedDataIds = default(int[]),
                    subfolderIds = new int[] { 2 }
                });

                Assert.IsTrue(JToken.DeepEquals(JToken.Parse(content), expected));
            }
        }

        [TestMethod]
        public async Task GetProtectedDataWithProvidingAnId_UserHasReadPermissions_ShouldSucceed()
        {
            ProtectedData data = new ProtectedData()
            {
                Name = "Data",
                Data = Encoding.UTF8.GetBytes("DATA")
            };
            data.Users.RegisterEntries.Add(new UserRegisterEntry()
            {
                User = User,
                Permission = Permission.READ,
                EncryptedKeys = new[]
                {
                    new SymmetricKey() { KeyData = Encoding.UTF8.GetBytes("SymmetricKey1"), MatchingPublicKeyId = 0 },
                    new SymmetricKey() { KeyData = Encoding.UTF8.GetBytes("SymmetricKey2"), MatchingPublicKeyId = 1 }
                }.ToList()
            });

            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(data);

            HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();
            DataModel dataModel = JsonConvert.DeserializeObject<DataModel>(content);

            Assert.AreEqual(data.Name, dataModel.Name);
            CollectionAssert.AreEquivalent(data.Data, dataModel.Data);
            CollectionAssert.AreEquivalent(User.PublicKeys[0].KeyData, dataModel.KeyPairs.ToList()[0].PublicKey);
            CollectionAssert.AreEquivalent(User.PublicKeys[1].KeyData, dataModel.KeyPairs.ToList()[1].PublicKey);
            CollectionAssert.AreEquivalent(data.Users.RegisterEntries[0].EncryptedKeys[0].KeyData, dataModel.KeyPairs.ToList()[0].SymmetricKey);
            CollectionAssert.AreEquivalent(data.Users.RegisterEntries[0].EncryptedKeys[1].KeyData, dataModel.KeyPairs.ToList()[1].SymmetricKey);
        }

        [TestMethod]
        public async Task GetProtectedDataWithProvidingAnId_UserHasNoPermissions_ShouldFailWithStatusCode403()
        {
            ProtectedData data = new ProtectedData()
            {
                OwnerId = 1234
            };
            ProtectedDataRepository.Setup(x => x.GetById(999)).Returns(data);

            HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/data/999");

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task GetProtectedDataWithProvidingAnIdThatDoesNotExist_ShouldFailWithStatusCode404()
        {
            HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/data/999");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetProtectedDataWithProvidingAnId_UserHasViewPermissions_ShouldFailWithStatusCode403()
        {
            ProtectedData data = new ProtectedData()
            {
                OwnerId = 999
            };
            data.Users.RegisterEntries.Add(new UserRegisterEntry() {
                User = User,
                Permission = Permission.VIEW
            });
            ProtectedDataRepository.Setup(x => x.GetById(1)).Returns(data);

            HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/data/1");

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task AddProtectedDataToRootFolder_ShouldSucceed()
        {
            Folder rootFolder = new Folder();
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
            ProtectedDataRepository.Setup(x => x.GetFolderById(null)).Returns(rootFolder);
            ProtectedDataRepository.Setup(x => x.AddToFolder(It.IsAny<ProtectedData>(), rootFolder)).Returns(unit.Object).Verifiable();

            HttpResponseMessage response = await UserClient.PostAsJsonAsync("/api/protecteddata/data", model);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("/api/protecteddata/data/0", response.Headers.Location.ToString());
            ProtectedDataRepository.Verify(x => x.AddToFolder(It.IsAny<ProtectedData>(), rootFolder));
            unit.Verify(x => x.Complete());
        }

        [TestMethod]
        public async Task AddProtectedDataToExplicitFolder_ShouldSucceed()
        {
            Folder folder = new Folder();

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
            ProtectedDataRepository.Setup(x => x.AddToFolder(It.IsAny<ProtectedData>(), folder)).Returns(unit.Object).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetFolderById(1)).Returns(folder);

            HttpResponseMessage response = await UserClient.PostAsJsonAsync("/api/protecteddata/data/1", model);

            ProtectedDataRepository.Verify(x => x.AddToFolder(It.IsAny<ProtectedData>(), folder));
            unit.Verify(x => x.Complete());
        }

        [TestMethod]
        public async Task UpdateProtectedData_UserHasWritePermission_ShouldSucceed()
        {
            ProtectedData data = new ProtectedData()
            {
                OwnerId = 999,
                Name = "NewData",
                Data = Encoding.UTF8.GetBytes("OldData")
            };
            data.Users.RegisterEntries.Add(new UserRegisterEntry() {
                User = User,
                Permission = Permission.WRITE // User now has write permission and should be able to update
            });

            Mock <IUnitOfWork<ProtectedData>> unit = new Mock<IUnitOfWork<ProtectedData>>();
            unit.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(data);
            ProtectedDataRepository.Setup(x => x.Update(It.IsAny<ProtectedData>())).Returns(unit.Object).Verifiable();

            HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();

            dynamic requestedData = JToken.Parse(content);
            requestedData.Data = Encoding.UTF8.GetBytes("NewData");

            response = await UserClient.PutAsJsonAsync("/api/protecteddata/data/0", (JToken) requestedData);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            ProtectedDataRepository.Verify(x => x.Update(It.IsAny<ProtectedData>()));
            unit.Verify(x => x.Complete());

            Assert.AreEqual("NewData", Encoding.UTF8.GetString(data.Data));
        }

        [TestMethod]
        public async Task UpdateProtectedData_UserHasReadPermission_ShouldFail()
        {
            ProtectedData data = new ProtectedData()
            {
                OwnerId = 999,  
                Name = "NewData",
                Data = Encoding.UTF8.GetBytes("OldData")
            };
            data.Users.RegisterEntries.Add(new UserRegisterEntry()
            {
                User = User,
                Permission = Permission.READ // User only has read permission and should not be able to update
            });

            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(data);

            HttpResponseMessage response = await UserClient.GetAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            string content = await response.Content.ReadAsStringAsync();

            dynamic requestedData = JToken.Parse(content);
            requestedData.Data = Encoding.UTF8.GetBytes("NewData");

            response = await UserClient.PutAsJsonAsync("/api/protecteddata/data/0", (JToken)requestedData);
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task RemoveProtectedData_UserIsOwner_ShouldSucceed()
        {
            ProtectedData data = new ProtectedData()
            {
                OwnerId = User.Id
            };

            Mock<IUnitOfWork<ProtectedData>> uow = new Mock<IUnitOfWork<ProtectedData>>();
            uow.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetById(1)).Returns(data);
            ProtectedDataRepository.Setup(x => x.Remove(data)).Returns(uow.Object).Verifiable();

            HttpResponseMessage response = await UserClient.DeleteAsync("/api/protecteddata/data/1");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            ProtectedDataRepository.Verify(x => x.Remove(data));
            uow.Verify(x => x.Complete());
        }

        [TestMethod]
        public async Task RemoveProtectedData_UserHasReadPermissionShouldFail()
        {
            ProtectedData data = new ProtectedData()
            {
                OwnerId = 999
            };
            data.Users.RegisterEntries.Add(new UserRegisterEntry()
            {
                User = User,
                Permission = Permission.READ
            });

            Mock<IUnitOfWork<ProtectedData>> uow = new Mock<IUnitOfWork<ProtectedData>>();
            uow.Setup(x => x.Complete()).Verifiable();
            ProtectedDataRepository.Setup(x => x.GetById(0)).Returns(data);
            ProtectedDataRepository.Setup(x => x.Remove(data)).Returns(uow.Object).Verifiable();

            HttpResponseMessage response = await UserClient.DeleteAsync("/api/protecteddata/data/0");
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
            uow.VerifyNoOtherCalls();
        }
        #endregion
    }
}