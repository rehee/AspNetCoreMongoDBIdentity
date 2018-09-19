using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public IActionResult Page()
    {
      var page = new Home();
      var context = new ValidationContext(page, null, null);
      var result = new List<ValidationResult>();

      // Act
      var valid = Validator.TryValidateObject(page, context, result, true);


      var model = ((ContentBase)page).ConvertToPassingModel();
      var page2 = db.GetContent("04d44aab-6afa-4d7b-b821-b6075c7628a7");
      return View(model);
    }
    [HttpPost]
    public IActionResult Page(ContentPostModel model)
    {
      var page = model.ConvertToBaseModel();
      page.AddContent();
      return View(model);
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
