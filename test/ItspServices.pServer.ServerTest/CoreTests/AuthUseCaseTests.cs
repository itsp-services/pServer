using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Response;
using ItspServices.pServer.Abstraction.UseCase;
using ItspServices.pServer.Abstraction.UseCase.Account;
using ItspServices.pServer.Core.Account;
using Moq;

namespace ItspServices.pServer.ServerTest.CoreTests
{
    [TestClass]
    public class AuthUseCaseTests
    {

        #region nested classes
        class MockOutputPort : IOutputPort<UseCaseResponse>
        {
            public bool Success { get; private set; }
            public string Message { get; private set; }

            public void Handle(UseCaseResponse response)
            {
                Success = response.Success;
                Message = response.Message;
            }
        }
        #endregion

        [TestMethod]
        public async Task UserCanLogin_ShouldSucceed()
        {
            var signInManager = new Mock<ISignInManager>();
            signInManager.Setup(x => x.PasswordSignInAsync(It.Is<string>(s => s == "fooUser"),
                                                           It.Is<string>(s => s == "barPassword")))
                .Returns(Task.FromResult(true));
            MockOutputPort mockOutputPort = new MockOutputPort();

            ILoginUserUseCase loginUseCase = new LoginUserUseCase(signInManager.Object);
            await loginUseCase.Handle(new LoginRequest("fooUser", "barPassword"), mockOutputPort);

            Assert.IsTrue(mockOutputPort.Success);
        }

        [TestMethod]
        public async Task UserCannotLogin_WrongPassword_ShouldFail()
        {
            var signInManager = new Mock<ISignInManager>();
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(),
                                                           It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            MockOutputPort mockOutputPort = new MockOutputPort();

            ILoginUserUseCase loginUseCase = new LoginUserUseCase(signInManager.Object);
            await loginUseCase.Handle(new LoginRequest("fooUser", "barPassword"), mockOutputPort);

            Assert.IsFalse(mockOutputPort.Success);
        }

        [TestMethod]
        public async Task UserCanLogout_ShouldSucceed()
        {
            var signInManager = new Mock<ISignInManager>();
            signInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(),
                                                           It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            signInManager.Setup(x => x.SignOutAsync())
                .Returns(Task.FromResult(true));
            MockOutputPort mockOutputPort = new MockOutputPort();

            ILoginUserUseCase loginUseCase = new LoginUserUseCase(signInManager.Object);
            ILogoutUserUseCase logoutUseCase = new LogoutUserUseCase(signInManager.Object);

            await loginUseCase.Handle(new LoginRequest("fooUser", "barPassword"), mockOutputPort);
            Assert.IsTrue(mockOutputPort.Success);

            await logoutUseCase.Handle(new LogoutRequest(), mockOutputPort);
            Assert.IsTrue(mockOutputPort.Success);
        }
    }
}
