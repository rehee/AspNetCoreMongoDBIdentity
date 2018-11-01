using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SDHCC;

namespace SDHCCContent.Views.Test
{
  public class FilesController : Controller
  {
    public IActionResult Index()
    {
      var file = $"{ContentE.FileUploadPath}\\33a35719-47fc-49f6-a9e0-f5d505a34925.bmp";
      file.DeleteFile(out var deleted);
      return View();
    }
    [HttpPost]
    public IActionResult Index(IFormFile fileup)
    {
      fileup.Save(out var path);
      return View();
    }
  }
}