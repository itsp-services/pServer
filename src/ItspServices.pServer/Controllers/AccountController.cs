using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ItspServices.pServer.Models;
using ItspServices.pServer.Presenter;
using ItspServices.pServer.Abstraction.UseCase.Account;
using ItspServices.pServer.Abstraction.Models.UseCase.Request.Account;

namespace ItspServices.pServer.Controllers
{
    public class AccountController : Controller
    {
        private readonly DefaultPresenter _useCaseOutputPort;
        private readonly ILoginUserUseCase _loginUserUseCase;
        private readonly ILogoutUserUseCase _logoutUserUseCase;
        private readonly IRegisterUserUseCase _registerUserUseCase;

        public AccountController(ILoginUserUseCase loginUserUseCase, ILogoutUserUseCase logoutUserUseCase, IRegisterUserUseCase registerUserUseCase)
        {
            _useCaseOutputPort = new DefaultPresenter();
            _loginUserUseCase = loginUserUseCase;
            _logoutUserUseCase = logoutUserUseCase;
            _registerUserUseCase = registerUserUseCase;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm]LoginModel loginModel, [FromQuery]string returnUrl = null)
        {
            await _loginUserUseCase.Handle(new LoginRequest(loginModel.Username, loginModel.Password), _useCaseOutputPort);

            loginModel.Password = null;
            ViewData["ReturnUrl"] = returnUrl;

            if (_useCaseOutputPort.Success)
            {
                return Redirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, _useCaseOutputPort.Message);
            return View(loginModel);
        }

        [HttpGet]
        public IActionResult Register(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm]RegisterModel registerModel, [FromQuery]string returnUrl = null)
        {
            await _registerUserUseCase.Handle(new RegisterRequest(registerModel.Username, registerModel.Password), _useCaseOutputPort);
            if(!_useCaseOutputPort.Success)
            {
                ModelState.AddModelError(string.Empty, _useCaseOutputPort.Message);
                return View(registerModel);
            }

            await _loginUserUseCase.Handle(new LoginRequest(registerModel.Username, registerModel.Password), _useCaseOutputPort);
            if (_useCaseOutputPort.Success)
            {
                return Redirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, _useCaseOutputPort.Message);
            return View(registerModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout([FromQuery]string returnUrl = null)
        {
            await _logoutUserUseCase.Handle(new LogoutRequest(), _useCaseOutputPort);
            return Redirect(returnUrl ?? "/");
        }
    }
}