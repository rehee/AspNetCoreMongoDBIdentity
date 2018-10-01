using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC.Identity.Services;
using SDHCCContent.ViewModels;

namespace SDHCCContent.Controllers
{
  public class AccountController : Controller
  {
    private ISDHCCIdentity users;
    public AccountController(ISDHCCIdentity users)
    {
      this.users = users;
    }
    public IActionResult Index()
    {
      return View();
    }
    public IActionResult Login(string ReturnUrl)
    {
      var model = new LoginViewModel()
      {
        Login = "",
        Password = "",
        ReturnUrl = ReturnUrl
      };
      return View(model);
    }
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
      var userName = users.CheckPassword(model.Login, model.Password);
      return Redirect(model.ReturnUrl ?? "/");
    }
    public IActionResult SignOut()
    {
      users.SignOut();
      return Redirect("/");
    }
  }
}