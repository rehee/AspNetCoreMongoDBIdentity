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
      var user = db.Where<MUser>().First();
      var token = await userManager.GenerateChangeEmailTokenAsync(user, "x3iii133@gmail.com");
      var result = await userManager.ChangeEmailAsync(user, "x3iii133@gmail.com", token);

      return new ObjectResult(result);
    }
  }

}