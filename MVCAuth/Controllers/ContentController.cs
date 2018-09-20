﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SDHCC.DB.Content;

namespace MVCAuth.Controllers
{
  public class ContentController : Controller
  {
    public IActionResult Index(string names)
    {
      var urls = new List<string>();
      if (!String.IsNullOrEmpty(names))
      {
        urls = names.Split('/').ToList();
      }
      if (urls.Count <= 0)
      {
        var root = ContentBase.context.GetChildrenNode("").OrderBy(b => b.SortOrder).FirstOrDefault();
        if (root != null)
        {
          var page = ContentBase.context.GetContent(root.Id);
          return View($"Views/{page.GetType().Name}.cshtml");
        }
        return Content("404");
      }
      var rootPage = ContentBase.context.GetChildrenNode("").OrderBy(b => b.SortOrder).FirstOrDefault();
      if (rootPage == null)
      {
        return Content("404");
      }
      ContentBase selectPage = null;
      var checkRoot = false;
      foreach (var url in urls)
      {
        var currentPage = ContentBase.context.GetChildrenNode(rootPage.Id)
          .Where(b => b.Name == url).OrderBy(b => b.SortOrder).FirstOrDefault();

        if (currentPage != null || checkRoot == false)
        {
          selectPage = currentPage;
          rootPage = currentPage;
        }
        else
        {
          checkRoot = true;
          currentPage = ContentBase.context.GetChildrenNode("")
            .Where(b => b.Name == url)
            .OrderBy(b => b.SortOrder).FirstOrDefault();
          if (currentPage == null)
          {
            return Content("404");
          }
          rootPage = currentPage;
          selectPage = currentPage;
        }
      }
      if (selectPage == null)
      {
        return Content("404");
      }
      return View($"Views/{selectPage.GetType().Name}.cshtml");
    }
  }
}