using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC.Identity.Models.UserModels;
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
      var allUser = users.GetUserRoles();
      if (string.IsNullOrEmpty(id))
      {
        return View(allUser.ToList());
      }
      else
      {
        return View(allUser.Where(b => users.IsUserInRole(b.Name, id)).ToList());
      }
    }

    public IActionResult Detail(string id)
    {
      if (string.IsNullOrEmpty(id))
        return RedirectToAction("Index");
      var user = users.GetUserByName(id);
      if (user == null)
        return RedirectToAction("Index");
      return View(user.ConvertUserToPass());
    }
    [HttpPost]
    public IActionResult Detail(SDHCCUserPass model)
    {
      var user = users.GetUserByName();
      return Content("");
    }
    public IActionResult UserRoles(string id)
    {
      return Content("");
    }
    public IActionResult UserAddRoles(string id)
    {
      return Content("");
    }
    public IActionResult UserRemoveRoles(string id)
    {
      return Content("");
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

    public IActionResult AssignRoles()
    {
      return Json(users.GetRoles().ToList());
    }
    public IActionResult UnAssignRoles()
    {
      return Json(users.GetRoles().ToList());
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