using SDHCC.DB.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SDHCC.DB.Content
{
  public static partial class ContentExtend
  {
    public static ContentPostModel ConvertToPassingModel(this ContentBase input)
    {
      var result = new ContentPostModel();
      var resultProperty = result.GetType().GetProperties().Where(b => b.BaseProperty()).ToList();
      var properties = input.GetType().GetProperties();
      foreach (var p in properties)
      {
        if (p.BaseProperty())
        {
          var baseP = resultProperty.Where(b => b.Name == p.Name).FirstOrDefault();
          baseP.SetValue(result, p.GetValue(input));
          continue;
        }
        if (p.SkippedProperty())
          continue;
        result.Properties.Add(p.GetContentPropertyByPropertyInfo(input));
      }
      return result;
    }
    public static ContentBase ConvertToBaseModel(this ContentPostModel input)
    {
      var result = (ContentBase)input.ConvertBaseTypeToT(out var typeName, out var assemblyName);
      var properties = result.GetType().GetProperties();
      var baseProperty = input.GetType().GetProperties().Where(b => b.BaseProperty()).ToList();
      foreach (var p in properties)
      {
        try
        {
          if (p.BaseProperty())
          {
            var inputBaseProperty = baseProperty.Where(b => b.Name == p.Name).FirstOrDefault();
            if (inputBaseProperty == null)
              continue;
            p.SetValue(result, inputBaseProperty.GetValue(input));
            continue;
          }

          if (p.SkippedProperty())
            continue;
          p.SetPropertyValue(input, result);
        }
        catch { }

      }
      result.FullType = typeName;
      result.AssemblyName = assemblyName;
      return result;
    }

    public static ContentProperty GetContentPropertyByPropertyInfo(this PropertyInfo p, object input)
    {
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
      return new ContentProperty()
      {
        Key = p.Name,
        Value = postValue,
        EditorType = editorType,
        ValueType = propertyType.FullName,
        Title = displayTitle,
      };
    }
    public static Type GetTypeFromContent(this BaseTypeEntity input, out string typeName, out string assemblyName)
    {
      typeName = input.FullType.Split(',').FirstOrDefault().Trim();
      var assemblyNames = input.AssemblyName.Split(',').ToList();
      assemblyName = "";
      if (assemblyNames.Count == 1)
      {
        assemblyName = assemblyNames[0];
      }
      else
      {
        assemblyName = assemblyNames[1];
      }
      var typeString = $"{typeName},{assemblyName}";
      var type = Type.GetType(typeString);
      return type;
    }
    public static object ConvertBaseTypeToT(this BaseTypeEntity input, out string typeName, out string assemblyName)
    {
      var type = input.GetTypeFromContent(out typeName, out assemblyName);
      if (type == null)
        return null;
      var result = Activator.CreateInstance(type);
      var properties = result.GetType().GetProperties();
      foreach (var p in properties)
      {
        switch (p.Name)
        {
          case "FullType":
            p.SetValue(result, typeName);
            break;
          case "AssemblyName":
            p.SetValue(result, assemblyName);
            break;
          default:
            break;
        }
      }
      return result;
    }
    public static void SetPropertyValue(this PropertyInfo p, IPassModel input,object result)
    {
      var propertyPost = input.Properties.Find(b => b.Key == p.Name);
      if (propertyPost == null)
      {
        return;
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
    public static bool SkippedProperty(this PropertyInfo property)
    {

      if (property.CustomAttributes.Where(b => b.AttributeType == typeof(IgnoreEditAttribute)).FirstOrDefault() != null)
      {
        return true;
      }
      return false;
    }
    public static bool CustomProperty(this PropertyInfo property)
    {
      if (property.CustomAttributes.Where(b => b.AttributeType == typeof(CustomPropertyAttribute)).FirstOrDefault() != null)
      {
        return true;
      }
      return false;
    }
    public static bool BaseProperty(this PropertyInfo property)
    {

      if (property.CustomAttributes.Where(b => b.AttributeType == typeof(BasePropertyAttribute)).FirstOrDefault() != null)
      {
        return true;
      }
      return false;
    }
  }
}
