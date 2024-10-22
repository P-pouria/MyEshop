using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MyEshop.Data.Repositories;
using MyEshop.Models;

namespace MyEshop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        #region Register

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            //if (_userRepository.IsExistUserByEmail(register.Email.ToLower()))
            //{
            //    ModelState.AddModelError("Email", "ایمیل وارد شده قبلا ثبت نام کرده است");
            //    return View(register);
            //}

            var user = new Users
            {
                Email = register.Email.ToLower(),
                Password = register.Password,
                IsAdmin = false, // Assuming default role is not admin
                RegisterDate = DateTime.Now
            };

            _userRepository.AddUser(user);

            return View("SuccessRegister", register);
        }

        public IActionResult VerifyEmail(string email)
        {
            if (_userRepository.IsExistUserByEmail(email.ToLower()))
            {
                return Json($"ایمیل {email} تکراری است");
            }

            return Json(true);
        }

        #endregion

        #region Login

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = _userRepository.GetUserForLogin(login.Email.ToLower(), login.Password);
            if (user == null)
            {
                ModelState.AddModelError("Email", "اطلاعات صحیح نیست");
                return View(login);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString()) // Adding user role as a claim
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = login.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            return Redirect("/");
        }

        #endregion

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/Login");
        }
    }
}
