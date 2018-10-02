using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC.Identity.Services;

namespace SDHCCContent.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class UserController : Controller
  {
    ISDHCCIdentity users;
    public UserController(ISDHCCIdentity users)
    {
      this.users = users;
    }
    public IActionResult Index(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        var allUser = users.GetUserRoles().ToList();
        return View(allUser);
      }
      else
      {
        var allUser = users.GetUserRoles().ToList().Where(b => users.IsUserInRole(b.Name, id));
        return View(allUser);
      }

    }
    public IActionResult Login()
    {
      return View();
    }
    public IActionResult GetRoles()
    {
      return Json(users.GetRoles().ToList());
    }
    public IActionResult CreateRole()
    {
      users.AddRole("Admin", out var role);
      var r = role;
      return Content("");
    }
    [HttpPost]
    public IActionResult Login(string loginName, string password)
    {
      return View();
    }


    public IActionResult Logout()
    {

      return Redirect("/");
    }

  }
}