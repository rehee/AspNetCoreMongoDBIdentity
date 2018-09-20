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

namespace MVCAuth.Controllers
{
  public class PageController : Controller
  {
    ISDHCCDbContext db { get; set; }
    public PageController(ISDHCCDbContext db)
    {
      this.db = db;
    }
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

    public IActionResult PreCreate(string fullName, string parentId = "")
    {
      var type = Type.GetType(fullName);
      var newContent = (ContentBase)Activator.CreateInstance(type);
      newContent.ParentId = parentId;
      return View("Create", newContent.ConvertToPassingModel());
    }

    [HttpPost]
    public IActionResult Create(ContentPostModel model)
    {
      var content = model.ConvertToBaseModel();
      content.AddContent();
      content.Refresh();
      return View(content.ConvertToPassingModel());
    }

    [HttpPost]
    public IActionResult Save(ContentPostModel model)
    {
      var content = model.ConvertToBaseModel();
      content.UpdatePageContent();
      return RedirectToAction("Index", "page", new { @id = model.Id });
    }
  }
}