using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SDHCC.DB.Content
{
  public static class ContentExtend
  {
    public static ContentPostModel ConvertToPassingModel(this ContentBase input)
    {
      var result = new ContentPostModel();
      result.FullType = input.FullType;
      result.Id = input.Id;
      result.ParentId = input.ParentId;
      result.AssemblyName = input.AssemblyName;

      var properties = input.GetType().GetProperties();
      foreach (var p in properties)
      {
        if (p.SkippedProperty())
          continue;
        var editorType = EnumInputType.Text;
        var typeAttribute = p.CustomAttributes.Where(b => b.AttributeType == typeof(InputTypeAttribute)).FirstOrDefault();
        if (typeAttribute != null)
        {
          editorType = (EnumInputType)typeAttribute.ConstructorArguments.FirstOrDefault().Value;
        }
        var propertyType = p.GetType();
        var displayTitle = p.Name.SpacesFromCamel();
        var displayAttribute = p.CustomAttributes.Where(b => b.AttributeType == typeof(DisplayAttribute)).FirstOrDefault();
        if (displayAttribute != null)
        {
          foreach (var item in displayAttribute.NamedArguments)
          {
            switch (item.MemberName)
            {
              case "Name":
                displayTitle = item.TypedValue.Value.ToString();
                break;
              default:
                break;
            }
          }
        }
        string postValue = "";
        if (ConvertTypeToStringDictionary.ContainsKey(p.GetType()))
        {
          postValue = ConvertTypeToStringDictionary[p.GetType()](p.GetValue(input));
        }
        else
        {
          postValue = p.GetValue(input) != null ? p.GetValue(input).ToString() : "";
        }

        result.Properties.Add(new ContentProperty()
        {
          Key = p.Name,
          Value = postValue,
          EditorType = editorType,
          ValueType = propertyType.FullName,
          Title = displayTitle,
        });
      }
      return result;
    }
    public static ContentBase ConvertToBaseModel(this ContentPostModel input)
    {
      var type = Type.GetType($"{input.FullType},MVCAuth");
      var result = (ContentBase)Activator.CreateInstance(type);
      result.FullType = input.FullType;
      result.Id = input.Id;
      result.ParentId = input.ParentId;
      result.AssemblyName = input.AssemblyName;

      var properties = result.GetType().GetProperties();
      foreach (var p in properties)
      {
        try
        {
          if (p.SkippedProperty())
            continue;
          var propertyPost = input.Properties.Find(b => b.Key == p.Name);
          if (propertyPost == null)
          {
            continue;
          }
          dynamic value = null;
          var stringValue = "";
          if (!String.IsNullOrEmpty(propertyPost.Value))
          {
            stringValue = propertyPost.Value;
          }
          var keyType = p.PropertyType;
          if (ConvertStringToTypeDictionary.ContainsKey(keyType))
          {
            value = ConvertStringToTypeDictionary[keyType](stringValue);
          }
          else
          {
            value = stringValue;
          }

          p.SetValue(result, value);
        }
        catch { }

      }
      return result;
    }
    public static bool SkippedProperty(this PropertyInfo property)
    {
      switch (property.Name)
      {
        case "FullType":
        case "Id":
        case "ParentId":
        case "Children":
        case "AssemblyName":
          return true;
        default:
          break;
      }
      if (property.CustomAttributes.Where(b => b.AttributeType == typeof(IgnoreEditAttribute)).FirstOrDefault() != null)
      {
        return true;
      }

      var a = property;
      return false;
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

    public static Dictionary<Type, Func<string, dynamic>> ConvertStringToTypeDictionary { get; set; } = new Dictionary<Type, Func<string, dynamic>>()
    {
      [typeof(int)] = b =>
      {
        int.TryParse(b, out var result);
        return result;
      },
    };
    public static Dictionary<Type, Func<object, string>> ConvertTypeToStringDictionary { get; set; } = new Dictionary<Type, Func<object, string>>()
    {
      [typeof(string)] = b =>
      {
        if (b == null)
        {
          return "";
        }
        return b.ToString();
      },
      [typeof(int)] = b =>
      {
        if (b == null)
        {
          return "0";
        }
        return b.ToString();
      },
    };
  }
}
