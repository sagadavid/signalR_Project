using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using signalR_Project.Models;
using System;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace signalR_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //print the access token into the application console once we log in,
            //so we can copy and paste it into request headers to make it possible to authenticate from other client applications
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            Console.WriteLine($"Access token: {accessToken}");
            return View();
        }

        public IActionResult WebAssemblyClient()
        {
            return View();

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LogOut()
        {
            return new SignOutResult(new[] { CookieAuthenticationDefaults.AuthenticationScheme, "oidc" });
        }
    }
}
