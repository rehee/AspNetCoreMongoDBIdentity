using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SDHCC;
using SDHCC.DB.Content;
using SDHCC.DB.Models;
using SDHCC.Identity.Models.UserModels;

namespace SDHCCContent.Controllers
{
  public class TestController : Controller
  {
    public string Index()
    {
      var test = new Test();
      var tPass = test.ConvertUserToPass();
      return "";
    }
  }

  public class Test : SDHCCUserBase
  {
    [CustomProperty]
    public string Avata { get; set; } 
  }
}