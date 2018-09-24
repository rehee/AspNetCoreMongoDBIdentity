using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SDHCC.DB.Content
{
  public static partial class ContentExtend
  {
    public static void AddContent(this ContentBase input)
    {
      if (input == null)
        return;
      ContentBase.context.AddContent(input);
    }
    public static void MoveTo(this ContentBase input, ContentBase target)
    {
      if (input == null || target == null)
        return;
      ContentBase.context.MoveContent(input, target);
    }
    public static void UpdatePageContent(this ContentBase input)
    {
      if (input == null)
        return;
      ContentBase.context.UpdatePageContent(input);
    }
    public static T Refresh<T>(this T input) where T : ContentBase
    {
      if (input == null)
        return null;
      var content = ContentBase.context.GetContent(input.Id);
      if (content == null)
      {
        return default(T);
      }
      return (T)content;
    }
    public static ContentBase Parent(this ContentBase input)
    {
      if (input == null)
        return null;
      return ContentBase.context.Find<ContentBase>(input.ParentId, "ContentBase", out var r);
    }
    public static IQueryable<ContentBase> Children(this ContentBase input)
    {
      if (input == null)
        return Enumerable.Empty<ContentBase>().AsQueryable();
      return ContentBase.context.Find<ContentBase>(input.Children, "ContentBase", out var r);
    }
    public static IQueryable<BsonDocument> ChildrenNode(this ContentBase input)
    {
      if (input == null)
        return Enumerable.Empty<BsonDocument>().AsQueryable();
      return ContentBase.context.Where(b => b["_id"] == input.Id, "ContentBase");
    }
    public static IEnumerable<ContentBase> Breadcrumb(this ContentBase input)
    {
      if (input == null)
      {
        return Enumerable.Empty<ContentBase>();
      }
      return ContentBase.context.GetBreadcrumb(input.Id);
    }
  }
}
