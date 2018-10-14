using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
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
        var prop = p.GetContentPropertyByPropertyInfo(input);
        if (prop == null)
          continue;
        result.Properties.Add(prop);
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
      var multiSelect = false;
      Type relatedType = null;
      var inputAttribute = p.GetCustomAttribute<InputTypeAttribute>();
      if (inputAttribute != null)
      {
        editorType = inputAttribute.EditorType;
        multiSelect = inputAttribute.MultiSelect;
        relatedType = inputAttribute.RelatedType;
      }
      var propertyType = p.GetType();
      var displayTitle = p.Name.SpacesFromCamel();
      var displayAttribute = p.GetCustomAttribute<DisplayAttribute>();
      if (displayAttribute != null && String.IsNullOrEmpty(displayAttribute.Name))
      {
        displayTitle = displayAttribute.Name;
      }
      string postValue = "";
      var type = p.PropertyType;
      var datetimeType = typeof(DateTime);
      List<DropDownViewModel> selector = new List<DropDownViewModel>();
      if (editorType == EnumInputType.DropDwon)
      {
        if (type == null)
          return null;
        if (!multiSelect)
        {
          postValue = p.GetValue(input) != null ? p.GetValue(input).ToString() : "";
          if (p.PropertyType.IsEnum)
          {
            var enumValues = p.PropertyType.GetEnumValues();
            foreach(var item in enumValues)
            {
              var select = new DropDownViewModel();
              select.Name = item.ToString();
              select.Value = item.ToString();
              select.Select = postValue == select.Value;
              selector.Add(select);
            }
          }
          else
          {
            var allSelect = ContentBase.context.GetDropDownsByName(relatedType.Name);
            var selects = new List<DropDownViewModel>();
            foreach (var item in allSelect)
            {
              var select = new DropDownViewModel();
              select.Name = item.Name;
              select.Value = item.Id;
              select.Select = postValue == select.Value;
              selector.Add(select);
            }
          }
          
        }
        else
        {
          var selectValues = (dynamic)p.GetValue(input);
          var selectStrings = new List<string>();
          if (selectValues != null)
          {
            foreach (object item in selectValues)
            {
              selectStrings.Add(item.ToString());
            }
          }
          postValue = selectStrings.Count > 0 ? String.Join(",", selectStrings) : "";
        }
      }
      else
      {
        if (ConvertTypeToStringDictionary.ContainsKey(type))
        {
          postValue = ConvertTypeToStringDictionary[type](p.GetValue(input));
        }
        else
        {
          postValue = p.GetValue(input) != null ? p.GetValue(input).ToString() : "";
        }
      }

      return new ContentProperty()
      {
        Key = p.Name,
        Value = postValue,
        EditorType = editorType,
        ValueType = propertyType.FullName,
        Title = displayTitle,
        MultiSelect = multiSelect,
        SelectItems = editorType == EnumInputType.DropDwon? selector:Enumerable.Empty<DropDownViewModel>()
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
    public static void SetPropertyValue(this PropertyInfo p, IPassModel input, object result)
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
      var inputAttribute = p.GetCustomAttribute<InputTypeAttribute>();
      if (inputAttribute != null && inputAttribute.EditorType == EnumInputType.DropDwon)
      {
        if (!inputAttribute.MultiSelect)
        {
          if (p.PropertyType.IsEnum)
          {
            var enumValue = p.PropertyType.GetEnumValues();
            foreach (var item in enumValue)
            {
              if (item.ToString() == stringValue)
              {
                value = item;
                break;
              }
            }
          }
          else
          {
            value = stringValue;
          }
        }
        else
        {
          if (p.PropertyType.IsEnum)
          {
            var enumValue = p.PropertyType.GetEnumValues();
            var stringValues = stringValue.Split(',').ToList();
            var values = new List<dynamic>();
            stringValues.ForEach(s =>
            {
              foreach (var item in enumValue)
              {
                if (item.ToString() == s)
                {
                  values.Add(item);
                  break;
                }
              }
            });
            value = values;
          }
          else
          {
            value = stringValue.Split(',').ToList();
          }
        }
      }
      else if (ConvertStringToTypeDictionary.ContainsKey(keyType))
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
