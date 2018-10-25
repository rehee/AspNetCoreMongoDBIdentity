using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC;
using SDHCC.DB;
using SDHCC.DB.Content;

namespace SDHCC.Admins.Controllers
{
  public class ContentController : Controller
  {
    public IActionResult Index(string names)
    {
      try
      {
        ContentBase selectPage = null;
        var root = ContentBase.context.Where(b => b["ParentId"] == "", "ContentBase").OrderBy(b => b["SortOrder"]).FirstOrDefault().ConvertToContentBase();
        if (String.IsNullOrEmpty(names))
        {
          selectPage = root;
          if (selectPage == null)
          {
            goto GoTO404;
          }
          else
          {
            goto GoTOView;
          }
        }
        var urls = names.Split('/').Select(b => b.Trim().ToLower()).ToList();
        var contains = new List<ContentBase>();
        var rootId = "";
        var parentId = root.Id;
        foreach (var url in urls)
        {
          var content = ContentBase.context
            .Where(b => b["Name"] == url &&(b["ParentId"]==rootId || b["ParentId"] == parentId), "ContentBase")
            .OrderBy(b => b["SortOrder"]).FirstOrDefault().ConvertToContentBase();
          if (content == null)
          {
            goto GoTO404;
          }
          if (content.ParentId != rootId && content.ParentId != parentId)
          {
            goto GoTO404;
          }
          rootId = parentId;
          parentId = content.Id;
          contains.Add(content);
        }
        selectPage = contains.LastOrDefault();
        GoTOView:
        return View($"Views/{selectPage.GetType().Name}.cshtml");
        GoTO404:
        return Content("404");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return Content("");
      }
    }
  }
}