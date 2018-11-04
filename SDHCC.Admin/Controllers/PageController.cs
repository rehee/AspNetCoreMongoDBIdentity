using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SDHCC;
using SDHCC.DB;
using SDHCC.DB.Content;
using SDHCC.Identity.Models.UserModels;
using SDHCC.Identity.Services;

namespace SDHCC.Admins.Controllers
{
  [Area("Admin")]
  [Authorize]
  public class PageController : Controller
  {
    ISDHCCDbContext db { get; set; }
    private ISDHCCIdentity users;
    SiteSetting setting;
    public PageController(ISDHCCDbContext db, ISDHCCIdentity users)
    {
      this.db = db;
      setting = E.Setting;
      this.users = users;
    }
    public IActionResult Index(string id = "")
    {
      ContentPostModel model = null;
      if (!string.IsNullOrEmpty(id))
      {
        var content = db.GetContent(id);
        if (content != null && users.IsUserInRoles(User, content.AdminReadRoles, true))
        {
          model = content.ConvertToPassingModel();
        }
      }
      return View(model);
    }

    [HttpPost]
    public IActionResult PreCreate(string ContentId, string FullType)
    {
      if (string.IsNullOrEmpty(FullType))
      {
        return RedirectToAction("Index", "Page", new { area = "Admin", id = ContentId });
      }
      try
      {

        var contentType = Type.GetType(FullType);
        var contentPage = (ContentBase)Activator.CreateInstance(contentType);
        var parent = db.GetContent(ContentId);
        contentPage.ParentId = parent == null ? "" : parent.Id;
        var model = contentPage.ConvertToPassingModel();
        return View("Create", model);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return RedirectToAction("Index", "Page", new { area = "Admin", id = ContentId });
      }

    }
    [HttpPost]
    public IActionResult Create(ContentPostModel model, IEnumerable<string> multi)
    {
      var content = model.ConvertToBaseModel();
      content.AddContent();
      return RedirectToAction("Index", "Page", new { area = "Admin", id = content.Id });
    }

    public IActionResult Edit(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        return RedirectToAction("Index", "Page", new { area = "Admin", id = "" });
      }
      var content = db.GetContent(id);
      if (content == null)
      {
        return RedirectToAction("Index", "Page", new { area = "Admin", id = "" });
      }
      return View(content.ConvertToPassingModel());
    }
    [HttpPost]
    public IActionResult Edit(ContentPostModel model, IEnumerable<string> multi)
    {
      if (model == null)
      {
        return RedirectToAction("Index", "Page", new { area = "Admin", id = "" });
      }
      var content = model.ConvertToBaseModel();
      content.UpdatePageContent();
      return RedirectToAction("Index", "Page", new { area = "Admin", id = content.Id });
    }

    public IActionResult Sort(string id)
    {
      if (string.IsNullOrEmpty(id))
      {
        return View(null);
      }
      var content = db.GetContent(id);
      if (content == null)
      {
        return View(null);
      }
      return View(content.ConvertToPassingModel());
    }
    [HttpPost]
    public IActionResult Delete(string ContentId)
    {
      if (string.IsNullOrEmpty(ContentId))
      {
        return RedirectToAction("Index", "Page", new { area = "Admin", id = "" });
      }
      var content = db.GetContent(ContentId);
      if (content == null)
      {
        return RedirectToAction("Index", "Page", new { area = "Admin", id = "" });
      }
      db.RemoveContent(content);
      return RedirectToAction("Index", "Page", new { area = "Admin", id = content.ParentId });
    }

    public JsonResult GetChildren(string id = "")
    {
      var c = db.GetChildrenNode(id).Count();
      var children = db.GetChildrenNode(id)
        .Select(b => new { id = b.GetValueByKey("_id"), sortOrder = b.GetValueByKey("SortOrder") }).OrderByDescending(b => b.sortOrder).Skip(100)
        .ToList();
      return Json(children);
    }
    public JsonResult GetSortChildren(string id = "")
    {
      var c = db.GetChildrenNode(id).Count();
      var children = db.GetChildrenNode(id)
        .Select(b =>
          new
          {
            DT_RowId = "row_" + b.GetValueByKey("SortOrder").ToString(),
            title = b.GetValueByKey("_id") + "_1",
            readingOrder = b.GetValueByKey("SortOrder").ToString()
          }).OrderBy(b => b.readingOrder).ToList();
      var result = new { data = children };
      var json = Json(result);
      return json;
    }
  }
}