using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC.Identity.Models.UserModels;
using SDHCC.Identity.Services;

namespace SDHCC.Admins.Controllers
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

    public IActionResult Create()
    {
      var model = users.GetEmptyUser().ConvertUserToPass();
      return View(model);
    }
    [HttpPost]
    public IActionResult Create(SDHCCUserPass model, IEnumerable<string> selectRoles)
    {
      var user = model.ConvertPassToUser();
      users.CreateUser(user);
      if (!String.IsNullOrEmpty(user.Id))
      {
        users.UpdateUserRole(user, selectRoles);
      }
      else
      {
        return View(model);
      }
      return RedirectToAction("Detail", "User", new { @area = "Admin", @id = user.Id });
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
    public IActionResult Detail(SDHCCUserPass model, IEnumerable<string> selectRoles)
    {
      var user = users.GetUserById(model.Id);
      var userModel = user.ConvertUserToPass();
      userModel.Properties = model.Properties;
      var userUpdate = userModel.ConvertPassToUser();
      users.UpdateUser(userUpdate);
      users.UpdateUserRole(userUpdate, selectRoles);
      return RedirectToAction("Detail", "User", new { @area = "Admin", @id = user.NormalizedUserName });
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
    public IActionResult Roles()
    {
      return View();
    }
    [HttpPost]
    public IActionResult CreateRole(string roles)
    {
      if (string.IsNullOrEmpty(roles))
        goto gotoRolePage;
      var roleList = roles.Trim().Split(',').Select(b => b.Replace(" ", ""));
      users.AddRoles(roleList);
      gotoRolePage:
      return RedirectToAction("Roles");
    }
    [HttpPost]
    public IActionResult RemoveRole(IEnumerable<string> roles)
    {
      users.RemoveRoles(roles);
      return RedirectToAction("Roles");
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