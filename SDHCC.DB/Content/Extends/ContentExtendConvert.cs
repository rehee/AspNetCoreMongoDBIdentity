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
      var assemblyNames = input.AssemblyName.Split(',').ToList();
      var assemblyName = "";
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
      var result = (ContentBase)Activator.CreateInstance(type);
      result.FullType = typeName;
      result.Id = input.Id;
      result.ParentId = input.ParentId;
      result.AssemblyName = assemblyName;
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

      if (property.CustomAttributes.Where(b => b.AttributeType == typeof(IgnoreEditAttribute)).FirstOrDefault() != null)
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
