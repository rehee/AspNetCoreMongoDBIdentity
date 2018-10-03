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
      var test = new TestUser();
      test.UserName = "123";
      test.Email = "123";
      var tPass = test.ConvertUserToPass();
      var convertBack = tPass.ConvertPassToUser();
      return "";
    }
  }

  public class TestUser : SDHCCUserBase
  {
    [CustomProperty]
    public string Avata { get; set; } 
  }
}