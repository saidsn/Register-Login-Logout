using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Partial_views__Load_more.Models;
using Partial_views__Load_more.ViewModels.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Partial_views__Load_more.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }


            AppUser user = new AppUser
            {
                FullName = registerVM.FullName,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                   
                }
                return View();
            }
            //await _signInManager.SignInAsync(user,false);

            return RedirectToAction(nameof(Login));

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            AppUser user = await _userManager.FindByEmailAsync(loginVM.UserNameorEmail);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UserNameorEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Email or Password wrong");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password wrong");
                return View(loginVM);
            }

            return RedirectToAction("Index", "Accessories");

        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Accessories");
        }

    }
}
