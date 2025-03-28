using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;

namespace MvcOAuthEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceEmpleados service;
        public ManagedController(ServiceEmpleados service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            string token = await this.service
            .GetTokenAsync(model.UserName, model.Password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
                return View();
            }
            else
            {
                ViewData["MENSAJE"] = "Ya tienes tu Token!!!";
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity =
                new ClaimsIdentity
                (CookieAuthenticationDefaults.AuthenticationScheme
                , ClaimTypes.Name, ClaimTypes.Role);
                //ALMACENAMOS EL NOMBRE DEL USUARIO (BONITO) 
                identity.AddClaim(new Claim
    (ClaimTypes.Name, model.UserName));
                //EN ESTE EJEMPLO, ALMACENAMOS EL ID (PASSWORD) 
                identity.AddClaim(new Claim
    (ClaimTypes.NameIdentifier, model.Password));
                identity.AddClaim(new Claim
    ("TOKEN", token));
                ClaimsPrincipal principal =
                new ClaimsPrincipal(identity);
                //EL USUARIO ESTARA DADO DE ALTA 30 MINUTOS 
                //, LO MISMO QUE SESSION  
                await HttpContext.SignInAsync
    (CookieAuthenticationDefaults.AuthenticationScheme
    , principal, new AuthenticationProperties
    {
        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
    });
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    
}
}
