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
    public static ContentPostModel ConvertToPassingModel(this ContentBase input)
    {
      var result = new ContentPostModel();
      result.FullType = input.FullType;
      result.Id = input.Id;
      result.ParentId = input.ParentId;
      result.AssemblyName = input.AssemblyName;
      result.CreateTime = input.CreateTime;
      result.SortOrder = input.SortOrder;
      result.Name = input.Name;

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
        var type = p.PropertyType;
        var datetimeType = typeof(DateTime);
        if (ConvertTypeToStringDictionary.ContainsKey(type))
        {
          postValue = ConvertTypeToStringDictionary[type](p.GetValue(input));
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
      var typeName = input.FullType.Split(',').FirstOrDefault().Trim();
      var typeString = $"{typeName},{input.AssemblyName}";
      var type = Type.GetType(typeString);
      var result = (ContentBase)Activator.CreateInstance(type);
      result.FullType = typeName;
      result.Id = input.Id;
      result.ParentId = input.ParentId;
      result.AssemblyName = input.AssemblyName;
      result.CreateTime = input.CreateTime;
      result.SortOrder = input.SortOrder;
      result.Name = input.Name;

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
        case "CreateTime":
        case "SortOrder":
        case "Name":
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
  }
}
