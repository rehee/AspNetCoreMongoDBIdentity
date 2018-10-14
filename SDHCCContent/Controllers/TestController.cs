using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
      var type = typeof(AAA);
      var value = Enum.GetValues(type);
      var value2 = type.GetEnumValues();
      //var property = type.GetProperties();
      var t = Enum.TryParse(type, "a", out var tt);
      var c = AAA.a == (AAA)tt;
      var p = type.GetEnumValues();
      foreach (var item in value2)
      {
        var order = item.GetObjectAttribute<int>("DisplayAttribute", "Order");
        Console.WriteLine("1");
      }


      return "";
    }
  }
  public enum AAA
  {
    [Display(Name = "111",Order =1)]
    [AllowChildren(ChildrenType = new Type[] { typeof(ttt) }, CreateRoles = new string[] { "a", "b", "c" })]
    a = 1,
    [Display(Name = "222")]
    b = 2,
    [Display(Name = "333")]
    c = 3,
  }
  public class ttt
  {
    public AAA a { get; set; } = AAA.b;
  }
  public class TestUser : SDHCCUserBase
  {
    [CustomProperty]
    public string Avata { get; set; }
  }
}