using DogGo.Models;
using DogGo.Models.ViewModels;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    public class AuthController : Controller
    {
        private IOwnerRepository _ownerRepo;
        private IWalkerRepository _walkerRepo;

        public AuthController(IOwnerRepository ownerRepo, IWalkerRepository walkerRepo)
        {
            _ownerRepo = ownerRepo;
            _walkerRepo = walkerRepo;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel)
        {
            List<Claim> claims = new List<Claim>();

            if (viewModel.Role == "Owner")
            {
                Owner owner = _ownerRepo.GetOwnerByEmail(viewModel.Email);

                if (owner == null)
                {
                    return Unauthorized();
                }

                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()),
                    new Claim(ClaimTypes.Email, owner.Email),
                    new Claim(ClaimTypes.Role, "DogOwner"),
                };
            }
            else if (viewModel.Role == "Walker")
            {
                //Walker walker = _walkerRepo.GetWalkerByEmail(viewModel.Email);

                //if (walker == null)
                //{
                //    return Unauthorized();
                //}

                //claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()),
                //    new Claim(ClaimTypes.Email, owner.Email),
                //    new Claim(ClaimTypes.Role, "DogOwner"),
                //};
            }
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
