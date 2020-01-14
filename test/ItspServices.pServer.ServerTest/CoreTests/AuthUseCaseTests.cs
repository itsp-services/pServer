using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Response;
using ItspServices.pServer.Abstraction.Repository;
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
        public void UserCanLogin_ShouldSucceed()
        {
            var userRepository = new Mock<IUserRepository>();

            ILoginUserUseCase loginUseCase = new LoginUserUseCase(userRepository.Object);

            MockOutputPort mockOutputPort = new MockOutputPort();

            loginUseCase.Handle(new LoginRequest("fooUser", "barPassword"), mockOutputPort);

            Assert.IsTrue(mockOutputPort.Success);
        }
    }
}
