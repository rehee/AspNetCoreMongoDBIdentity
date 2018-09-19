using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Content
{
  public static class ContentExtend
  {
    public static ContentPassingModel ConvertToPassingModel(this ContentBase input)
    {
      var result = new ContentPassingModel();
      result.FullType = input.FullType;
      result.Id = input.Id;
      result.ParentId = input.ParentId;

      var properties = input.GetType().GetProperties();
      foreach (var p in properties)
      {
        if (p.Name.SkippedProperty())
          continue;
        result.Properties.Add(p.Name, p.GetValue(input));
      }
      return result;
    }
    public static ContentBase ConvertToBaseModel(this ContentPassingModel input)
    {
      var type = Type.GetType(input.FullType);
      var result = (ContentBase)Activator.CreateInstance(type);
      result.FullType = input.FullType;
      result.Id = input.Id;
      result.ParentId = input.ParentId;

      var properties = result.GetType().GetProperties();
      foreach (var p in properties)
      {
        try
        {
          if (p.Name.SkippedProperty())
            continue;
          var value = input.Properties[p.Name];
          p.SetValue(result, value);
        }
        catch { }

      }
      return result;
    }
    public static bool SkippedProperty(this string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        return true;
      }
      switch (name.Trim())
      {
        case "FullType":
        case "Id":
        case "ParentId":
          return true;
        default:
          return false;
      }
    }

    public static void AddContent(this ContentBase input)
    {
      ContentBase.context.AddContent(input);
    }
    public static void MoveTo(this ContentBase input, ContentBase target)
    {
      ContentBase.context.MoveContent(input, target);
    }
    public static void UpdatePageContent(this ContentBase input)
    {
      var ignoreKeys = new List<string>()
      {
        "ParentId","Children","FullType",
      };
      ContentBase.context.UpdateContent(input, ignoreKeys);
    }
    public static T Refresh<T>(this T input) where T : ContentBase
    {
      var content = ContentBase.context.GetContent(input.Id);
      if (content == null)
      {
        return default(T);
      }
      return (T)content;
    }
  }
}
