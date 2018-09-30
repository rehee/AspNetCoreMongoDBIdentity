using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCAuth.Models;
using SDHCC.Identity.Services;

namespace MVCAuth.Controllers
{
  public class UserController : Controller
  {
    ISDHCCIdentity IdServices;
    public UserController(ISDHCCIdentity IdServices)
    {
      this.IdServices = IdServices;
    }
    public IActionResult Index()
    {
      var user = User.Identity.Name;
      return Content(this.IdServices.IsUserInRole(user, "aaa").ToString());
    }
  }
}