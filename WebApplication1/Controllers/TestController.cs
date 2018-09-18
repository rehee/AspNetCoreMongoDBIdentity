using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SDHCC.DB;
using SDHCC.DB.Models;
using SDHCC.DB.Orms;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
  [Route("[controller]/[action]")]
  public class TestController : Controller
  {
    private ISDHCCDbContext db;
    private UserManager<MUser> userManager;
    private RoleManager<MRole> roleManager;
    private ISDHCOrm o;
    public TestController(ISDHCCDbContext db, UserManager<MUser> userManager, ISDHCOrm o, RoleManager<MRole> roleManager)
    {
      this.db = db;
      this.userManager = userManager;
      this.o = o;
      this.roleManager = roleManager;
    }
    public async Task<IActionResult> Index()
    {
      var user = db.Where<MUser>().FirstOrDefault();
      //var token1 = await userManager.GenerateEmailConfirmationTokenAsync(user);
      //var result = await userManager.ConfirmEmailAsync(user, token1);
      //var result2 = await userManager.ConfirmEmailAsync(user, token1);
      //var token2 = await userManager.GenerateEmailConfirmationTokenAsync(user);
      //var token3 = await userManager.GenerateEmailConfirmationTokenAsync(user);
      //var result2 = await userManager.ConfirmEmailAsync(user, token1);
      //var token1 = await userManager.GenerateChangeEmailTokenAsync(user, "x3iii131@gmail.com");
      //var token2 = await userManager.GenerateChangeEmailTokenAsync(user, "x3iii132@gmail.com");
      var result = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 1);
      //var sets = await userManager.RedeemTwoFactorRecoveryCodeAsync(user, result.FirstOrDefault());
      var users = o.Where<MUser>().ToList();
      //var result = await userManager.GetUsersInRoleAsync("admin");
      return new ObjectResult(user);
      

    }

    public async Task<IActionResult> addRole()
    {
      await roleManager.CreateAsync(new MRole()
      {
        Id = Guid.NewGuid().ToString(),
        FullType = typeof(MRole).FullName,
        Name = "Admin",
        NormalizedName = "admin",
      });
      var role = roleManager.Roles.FirstOrDefault();
      var user = userManager.Users.FirstOrDefault();
      await userManager.AddToRoleAsync(user, "admin");
      return new ObjectResult(role);
    }
  }

  

}