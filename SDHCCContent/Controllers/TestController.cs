using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SDHCC;
using SDHCC.DB;
using SDHCC.DB.Content;
using SDHCC.DB.Models;
using SDHCC.Identity.Models.UserModels;
using SDHCC.Identity.Services;
using SDHCCContent.Models;
using SDHCCContent.Models.Enums;
using ContentBaseModel = SDHCCContent.Models.ContentBaseModel;

namespace SDHCCContent.Controllers
{
  public class TestController : Controller
  {
    ISDHCCDbContext db;
    ISDHCCIdentity us;
    public TestController(ISDHCCDbContext db, ISDHCCIdentity us)
    {
      this.db = db;
      this.us = us;
    }
    public string Index()
    {
      var isUser = us.IsUserInRoles(User, new List<string>() { "Admin", "Lalala" });
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
    public IActionResult File(string id)
    {
      id = "files\\" + id;
      return id.GetFileFromPath(this);
    }


  }
  public class Ti : BaseTypeEntity
  {
    public string AssemblyName { get; set; }
    public string Id { get; set; }
    public string FullType { get; set; }

    public string aa { get; set; }
  }
  public class TestPage : ContentBaseModel
  {
    [InputType(EditorType = EnumInputType.DropDwon, MultiSelect = true, RelatedType = typeof(EnumGender))]
    public IEnumerable<EnumGender> Gender { get; set; } =
      new List<EnumGender>()
      {
        EnumGender.Female, EnumGender.Male
      };
    public List<string> ABC { get; set; } = new List<string>();
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