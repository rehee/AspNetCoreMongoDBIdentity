using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SDHCC.DB.Content;
using SDHCC.DB.Models;

namespace SDHCCContent.Controllers
{
  public class TestController : Controller
  {
    public string Index()
    {
      //var test = new Test();
      //test.Id = Guid.NewGuid().ToString();
      //test.Roles = new List<string>() { "1", "2", "3" };
      //ContentBase.context.Add<Test>(test,out var r);
      var a = ContentBase.context.Where(b => ((BsonArray)b["Roles"]).Contains("1"), "Test").ToList();

      return "";
    }
  }

  public class Test : SDHCCBaseEntity
  {
    public IList<string> Roles { get; set; }
  }
}