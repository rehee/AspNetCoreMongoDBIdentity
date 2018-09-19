﻿using SDHCC.Core.MethodResponse;
using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC.DB
{
  public partial class SDHCCDbContext : ISDHCCDbContext
  {
    public string BaseContentType { get; set; } = "ContentBase";
    public string BaseContentTypeFullName { get; set; } = null;
    public void AddContent(ContentBase content)
    {
      MethodResponse response;
      if (string.IsNullOrEmpty(content.Id))
      {
        content.GenerateId();
      }
      var parent = GetContent(content.ParentId);
      if (parent == null)
      {
        content.ParentId = "";
      }
      Add<ContentBase>(content, BaseContentType, out response);
      if (parent != null && response.Success)
      {
        parent.Children.Add(content.Id);
        UpdateContent(parent, null, new string[] { "Children" });
      }
    }
    public void RemoveContent(ContentBase content)
    {
      RemoveContent(content.Id);
    }
    public void RemoveContent(string contentId)
    {
      MethodResponse response;
      if (String.IsNullOrEmpty(contentId))
      {
        return;
      }
      var content = GetContent(contentId);
      if (content == null)
      {
        return;
      }
      if (!String.IsNullOrEmpty(content.ParentId))
      {
        var parent = GetContent(content.ParentId);
        if (parent != null)
        {
          try
          {
            parent.Children.Remove(contentId);
          }
          catch { }
          UpdateContent(parent, null, new string[] { "Children" });
        }
      }
      foreach (var child in content.Children)
      {
        Remove<ContentBase>(null, BaseContentType, child);
      }
      Remove<ContentBase>(content, BaseContentType, content.Id);
    }

    public void MoveContent(ContentBase content, ContentBase target)
    {
      MoveContent(content.Id, target.Id);
    }
    public void MoveContent(ContentBase content, string target)
    {
      MoveContent(content.Id, target);
    }
    public void MoveContent(string contentId, ContentBase target)
    {
      MoveContent(contentId, target);

    }
    public void MoveContent(string contentId, string target)
    {
      MethodResponse response;
      if (String.IsNullOrEmpty(contentId))
      {
        return;
      }
      if (contentId.Equals(target))
      {
        return;
      }
      var content = GetContent(contentId);
      if (content == null)
      {
        return;
      }

      if (string.IsNullOrEmpty(target))
      {
        target = "";
      }
      else
      {
        var targetNode = GetContent(target);
        if (targetNode == null)
        {
          target = "";
        }
        else
        {
          try
          {
            targetNode.Children.Add(content.Id);
          }
          catch { }
          UpdateContent(targetNode, null, new string[] { "Children" });
        }
      }
      var parentId = content.ParentId;
      if (!string.IsNullOrEmpty(parentId))
      {
        var parentNode = GetContent(parentId);
        if (parentNode != null)
        {
          try
          {
            parentNode.Children.Remove(content.Id);
          }
          catch { }
          UpdateContent(parentNode, null, new string[] { "Children" });
        }
      }
      content.ParentId = target;
      UpdateContent(content, null, new string[] { "ParentId" });
    }

    public ContentBase GetContent(string id)
    {
      return (ContentBase)Find(id, BaseContentType, null, out var response);
    }
    public IEnumerable<ContentBase> GetContents(IEnumerable<string> ids)
    {
      return ids.Select(b => GetContent(b));
    }
    public IEnumerable<ContentBase> GetChildrenNode(string id)
    {
      var node = GetContent(id);
      if (node == null)
      {
        return Enumerable.Empty<ContentBase>();
      }
      return GetContents(node.Children);
    }

    public void UpdateContent(ContentBase content, IEnumerable<string> ignoreKeys = null, IEnumerable<string> takeKeys = null)
    {
      Update(content, content.Id, BaseContentType, ignoreKeys, takeKeys, out var response);
    }
  }
}