using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC;
using SDHCC.DB;
using SDHCC.DB.Content;

namespace SDHCCContent.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class PageController : Controller
  {
    //ISDHCCDbContext db { get; set; }
    //public PageController(ISDHCCDbContext db)
    //{
    //  //this.db = db;
    //}
    [ResponseCache(Duration = 1)]
    public IActionResult Index(string id = "")
    {
      ContentPostModel model = null;
      var allowChildrenType = new List<Type>();
      if (!string.IsNullOrEmpty(id))
      {
        var page = ContentBase.context.GetContent(id);
        if (page != null)
        {
          var currentType = Type.GetType($"{page.FullType},{page.AssemblyName}");
          var allowChildren = currentType.CustomAttributes.Where(b => b.AttributeType == typeof(AllowChildrenAttribute)).FirstOrDefault();
          if (allowChildren != null)
          {
            var childList = allowChildren.NamedArguments.Where(b => b.MemberName == "ChildrenType").FirstOrDefault();
            if (childList != null)
            {
              foreach (var t in (ReadOnlyCollection<CustomAttributeTypedArgument>)childList.TypedValue.Value)
              {
                var items = t.Value;
                allowChildrenType.Add((Type)items);
              }
            }
          }
          model = page.ConvertToPassingModel();
        }
      }
      else
      {
        var currentType = ContentE.RootType;
        var allowChildren = currentType.CustomAttributes.Where(b => b.AttributeType == typeof(AllowChildrenAttribute)).FirstOrDefault();
        if (allowChildren != null)
        {
          var childList = allowChildren.NamedArguments.Where(b => b.MemberName == "ChildrenType").FirstOrDefault();
          if (childList != null)
          {
            foreach (var t in (ReadOnlyCollection<CustomAttributeTypedArgument>)childList.TypedValue.Value)
            {
              var items = t.Value;
              allowChildrenType.Add((Type)items);
            }
          }
        }
      }
      ViewBag.allowChildrenType = allowChildrenType;
      return View(model);
    }

    public JsonResult GetChildren(string id = "")
    {
      var children = ContentBase.context.GetChildrenNode(id)
        .Select(b => new { id = b.GetValueByKey("_id"), sortOrder = b.GetValueByKey("SortOrder") })
        .ToList();
      return Json(children);
    }
  }
}