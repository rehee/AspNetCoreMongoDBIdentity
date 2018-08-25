using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SDHCC.DB;
using SDHCC.DB.Modules;

namespace WebApplication1.Controllers
{
  [Route("[controller]/[action]")]
  public class TestController : Controller
  {
    private ISDHCCDbContext db;
    private UserManager<MUser> userManager;
    public TestController(ISDHCCDbContext db, UserManager<MUser> userManager)
    {
      this.db = db;
      this.userManager = userManager;
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
      return new ObjectResult(user);
    }
  }

}