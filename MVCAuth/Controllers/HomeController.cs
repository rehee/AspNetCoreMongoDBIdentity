using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCAuth.Models;
using SDHCC.DB;
using SDHCC.DB.Content;

namespace MVCAuth.Controllers
{
  public class HomeController : Controller
  {
    private ISDHCCDbContext db;

    public HomeController(ISDHCCDbContext db)
    {
      this.db = db;
    }
    public IActionResult Index()
    {
      return View();
    }
    public string Page()
    {
      var page = new Home();
      var page2 = new Home2();
      page.Title = "title1";
      page2.Title2 = "title2";
      page.AddContent();
      page2.AddContent();
      page.MoveTo(page2);
      page2.Title2 = "new title 2";
      page2.UpdatePageContent();
      return "";
    }

    public IActionResult About()
    {
      ViewData["Message"] = "Your application description page.";

      return View();
    }

    public IActionResult Contact()
    {
      ViewData["Message"] = "Your contact page.";

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
  }
}
