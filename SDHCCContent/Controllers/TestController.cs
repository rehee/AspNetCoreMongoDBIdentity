using System;
using System.Collections;
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
using SDHCCContent.Models.Enums;

namespace SDHCCContent.Controllers
{
  public class TestController : Controller
  {
    public string Index()
    {
      var test = new List<string>();
      var a = test.GetType();
      var isEnum = a.IsIEnumerable();
      var eType = a.GetElementType() == null ? a.GetGenericArguments()[0] : a.GetElementType();
      return "";
    }
    public IActionResult Multi()
    {
      return View();
    }
    [HttpPost]
    public IActionResult Multi(IEnumerable<string> select)
    {
      return View();
    }
  }
  public class TestPage : ContentBaseModel
  {
    [InputType(EditorType = EnumInputType.DropDwon, MultiSelect = true, RelatedType = typeof(EnumGender))]
    public IEnumerable<EnumGender> Gender { get; set; } =
      new List<EnumGender>()
      {
        EnumGender.Female, EnumGender.Male
      };
  }
  public enum AAA
  {
    [Display(Name = "111", Order = 1)]
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