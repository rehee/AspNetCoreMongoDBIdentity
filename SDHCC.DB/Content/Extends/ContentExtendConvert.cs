using SDHCC.DB.Models;
using System;
using System.Collections;
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
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
        }

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
      var postMultiValue = new List<string>();
      List<DropDownViewModel> selector = new List<DropDownViewModel>();
      if (editorType == EnumInputType.DropDwon)
      {
        if (!multiSelect)
        {
          postValue = p.GetValue(input) != null ? p.GetValue(input).ToString() : "";
        }
        else
        {
          postValue = "";
          var selectValues = (dynamic)p.GetValue(input);
          if (selectValues != null)
          {
            foreach (object item in selectValues)
            {
              postMultiValue.Add(item.ToString());
            }
          }
        }
        p.SetDropDownSelect(selector, relatedType, postValue, postMultiValue);
      }
      else
      {
        //notmal value
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
        SelectItems = editorType == EnumInputType.DropDwon ? selector : Enumerable.Empty<DropDownViewModel>(),
        MultiValue = multiSelect ? postMultiValue : Enumerable.Empty<string>(),
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
    public static void SetDropDownSelect(this PropertyInfo p, List<DropDownViewModel> selector, Type relatedType, string postValue, IEnumerable<string> postValues = null)
    {
      List<String> values;
      if (postValues != null && postValues.Count() > 0)
      {
        values = postValues.ToList();
      }
      else
      {
        values = !String.IsNullOrEmpty(postValue) ? new List<string>() { postValue } : new List<string>();
      }
      if (p.PropertyType.IsEnum || p.PropertyType.GenericTypeArguments.Where(b => b.IsEnum).FirstOrDefault() != null)
      {
        Array enumValues;
        if (p.PropertyType.IsEnum)
        {
          enumValues = p.PropertyType.GetEnumValues();
        }
        else
        {

          enumValues = p.PropertyType.GenericTypeArguments.Where(b => b.IsEnum).FirstOrDefault().GetEnumValues();
        }
        foreach (var item in enumValues)
        {
          var select = new DropDownViewModel();
          select.Name = item.ToString();
          select.Value = item.ToString();
          select.Select = values.Contains(select.Value);
          selector.Add(select);
        }
      }
      else
      {
        //dropdown Classes
        var allSelect = ContentBase.context.GetDropDownsByName(relatedType.Name).ToList();
        var selects = new List<DropDownViewModel>();
        foreach (var item in allSelect)
        {
          var select = new DropDownViewModel();
          select.Name = item.Name;
          select.Value = item.Id;
          select.Select = values.Contains(select.Value);
          selector.Add(select);
        }
      }
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
          if (p.PropertyType.IsEnum || p.PropertyType.GenericTypeArguments.Where(b => b.IsEnum).FirstOrDefault() != null)
          {
            Array enumValue;
            Type enumType;
            if(p.PropertyType.IsEnum)
            {
              enumType = p.PropertyType;
              enumValue = p.PropertyType.GetEnumValues();
            }
            else
            {
              enumType = p.PropertyType.GenericTypeArguments.Where(b => b.IsEnum).FirstOrDefault();
              enumValue = enumType.GetEnumValues();

            }
            var stringValues = propertyPost.MultiValue.ToList();
            var values = new List<dynamic>();
            //var t = Activator.CreateInstance(p.PropertyType);
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(enumType);
            var instance = (IList)Activator.CreateInstance(constructedListType);
            stringValues.ForEach(s =>
            {
              try
              {
                instance.Add(Enum.Parse(enumType, s));
              }
              catch { }
            });
            
            p.SetValue(result, instance);
            return;
          }
          else
          {
            value = propertyPost.MultiValue.ToList();
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
