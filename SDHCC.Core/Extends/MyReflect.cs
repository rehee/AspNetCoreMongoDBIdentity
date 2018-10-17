using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
  public static class MyReflect
  {
    /// <summary>
    /// Get All CustomAttributeData from object as IEnumerable
    /// </summary>
    /// <param name="o">Object you want get CustomAttribute</param>
    /// <param name="getField">switch check fields attribute if it's Enum</param>
    /// <returns></returns>
    public static IEnumerable<CustomAttributeData> GetObjectCustomAttribute(this Object o, bool getField = true)
    {
      Type type = o.GetType();
      var customeAttribute = type.CustomAttributes;
      if (type.IsEnum && getField)
      {
        var info = o.GetType().GetField(o.ToString());
        customeAttribute = info.CustomAttributes;
      }
      return customeAttribute;
    }
    /// <summary>
    /// Retrive CustomAttribute from input object or projerty
    /// </summary>
    /// <typeparam name="T">CustomAttribute class try to Retrive</typeparam>
    /// <param name="o">Property or attribute try to get value</param>
    /// <param name="getField">switch check field attribute if the object is Enum. </param>
    /// <returns></returns>
    public static T GetObjectCustomAttribute<T>(this Object o, bool getField = true) where T : Attribute, new()
    {
      try
      {
        Type type = o.GetType();
        if (type.IsEnum && getField)
        {
          var info = o.GetType().GetField(o.ToString());
          return info.GetCustomAttribute<T>();
        }
        var t1 = o.GetType().GetCustomAttribute<T>();
        var t2 = o.GetType().GetCustomAttribute(typeof(T));
        return (T)t2;
      }
      catch { return default(T); }
    }
    public static T GetObjectCustomAttribute<T>(this PropertyInfo p, bool getField = true) where T : Attribute, new()
    {
      try
      {
        if (p.PropertyType.IsEnum && getField)
        {
          var info = p.PropertyType.GetType().GetField(p.Name);
          return info.GetCustomAttribute<T>();
        }
        var t1 = p.GetCustomAttribute<T>();
        return t1;
      }
      catch { return default(T); }
    }

    /// <summary>
    /// get DisplayAttribute from input object
    /// </summary>
    /// <param name="o">object to get DisplayAttribute</param>
    /// <param name="getField"></param>
    /// <returns>switch check field attribute if the object is Enum. </returns>
    public static DisplayAttribute GetObjectDisplayAttribute(this Object o, bool getField = true)
    {
      return o.GetObjectCustomAttribute<DisplayAttribute>(getField);
    }
    /// <summary>
    /// Try Get CustomAttribute field by Attribute Name And Field Name return null if there no custom attribute or field
    /// </summary>
    /// <param name="o">Object to heck custom attribute property</param>
    /// <param name="attributeName">the attribute checked</param>
    /// <param name="name">attribute property name</param>
    /// <param name="getField">switch check field attribute if the object is Enum. </param>
    /// <returns></returns>
    public static object GetObjectAttribute(this Object o, string attributeName, string name, bool getField = true)
    {
      Type type = o.GetType();
      var customeAttribute = type.CustomAttributes.Where(b => b.AttributeType.Name == attributeName).FirstOrDefault();
      if (type.IsEnum && getField)
      {
        var info = o.GetType().GetField(o.ToString());
        customeAttribute = info.CustomAttributes.Where(b => b.AttributeType.Name == attributeName).FirstOrDefault();
      }
      if (customeAttribute == null)
        return null;
      var nameAttribute = customeAttribute.NamedArguments.Where(b => b.MemberName == name).FirstOrDefault();
      if (nameAttribute == null)
        return null;
      return nameAttribute.TypedValue.Value;
    }
    /// <summary>
    /// Try Get Generic CustomAttribute field by Attribute Name And Field Name return null if there no custom attribute or field
    /// </summary>
    /// <typeparam name="T">Generic of the result</typeparam>
    /// <param name="o">Object to heck custom attribute property</param>
    /// <param name="attributeName">the attribute checked</param>
    /// <param name="name">attribute property name</param>
    /// <param name="getField">switch check field attribute if the object is Enum. </param>
    /// <returns></returns>
    /// <returns>switch check field attribute if the object is Enum. </returns>
    public static T GetObjectAttribute<T>(this Object o, string attributeName, string name, bool getField = true)
    {
      var customeAttribute = o.GetObjectCustomAttribute(getField)
        .Where(b => b.AttributeType.Name == attributeName).FirstOrDefault();
      if (customeAttribute == null)
        return default(T);
      var names = customeAttribute.NamedArguments.Where(b => b.MemberName == name).FirstOrDefault();
      if (names == null || names.TypedValue == null || names.TypedValue.Value == null)
        return default(T);
      return (T)names.TypedValue.Value;
    }
    /// <summary>
    /// Try Get List Generic CustomAttribute field by Attribute Name And Field Name return null if there no custom attribute or field
    /// </summary>
    /// <typeparam name="T">Generic of the result</typeparam>
    /// <param name="o">Object to heck custom attribute property</param>
    /// <param name="attributeName">the attribute checked</param>
    /// <param name="name">attribute property name</param>
    /// <param name="getField">switch check field attribute if the object is Enum. </param>
    /// <returns></returns>
    public static IEnumerable<T> GetObjectAttributes<T>(this Object o, string attributeName, string name, bool getField = true)
    {
      var customeAttribute = o.GetObjectCustomAttribute(getField)
        .Where(b => b.AttributeType.Name == attributeName).FirstOrDefault();
      if (customeAttribute == null)
        return Enumerable.Empty<T>();
      var names = customeAttribute.NamedArguments.Where(b => b.MemberName == name).FirstOrDefault();
      if (names == null || names.TypedValue == null || names.TypedValue.Value == null)
        return Enumerable.Empty<T>();
      var types = ((System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument>)names.TypedValue.Value)
            .Select(b => (T)b.Value).ToArray();
      return types;
    }
    /// <summary>
    /// Check the type is IEnumerable
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns></returns>
    public static bool IsIEnumerable(this Type type)
    {
      return typeof(IList).IsAssignableFrom(type);
    }
    /// <summary>
    /// Get Element Type from Ienumerable Type
    /// </summary>
    /// <param name="type">IEnumerable type to check</param>
    /// <returns></returns>
    public static Type GetIElementType(this Type type)
    {
      try
      {
        if (!type.IsIEnumerable())
          return null;
        return type.GetElementType() == null ? type.GetGenericArguments()[0] : type.GetElementType();
      }
      catch
      {
        return null;
      }
    }

    public static void SetPropertyValue(this PropertyInfo property, object result, object value)
    {
      if (value == null)
        return;
      var targetType = property.PropertyType;
      var valueType = value.GetType();
      if (targetType == valueType)
      {
        property.SetValue(result, value);
        return;
      }
      var convertResult = value.MyTryConvert(targetType);
      property.SetValue(result, convertResult);
    }

    public static Dictionary<Type, Func<string, object>> MyStringObjectConvertDictionary { get; set; } =
      new Dictionary<Type, Func<string, object>>();

    public static Dictionary<Type, Func<object, string>> MyObjectStringConvertDictionary { get; set; } =
      new Dictionary<Type, Func<object, string>>();

    public static object MyTryConvert(this string value, Type type)
    {
      if (MyStringObjectConvertDictionary.ContainsKey(type))
        return MyStringObjectConvertDictionary[type](value);
      var convertMethod = type.GetMethods().Where(b => b.Name == "" && b.GetParameters().Count() == 2).FirstOrDefault();
      if (convertMethod != null)
      {
        var parames = new object[] { value, null };
        var result = convertMethod.Invoke(null, parames);
        return parames[1];
      }
      try
      {
        return Convert.ChangeType(value, type);
      }
      catch { return null; }
    }
    public static object MyTryConvert(this object value, Type type)
    {
      if (value == null)
        return null;
      var valueType = value.GetType();
      if (valueType == typeof(string))
        return MyTryConvert((string)value, type);
      if (type == typeof(string))
      {
        if (MyObjectStringConvertDictionary.ContainsKey(valueType))
          return MyObjectStringConvertDictionary[valueType](value);
        return value.ToString();
      }
      try
      {
        return Convert.ChangeType(value, type);
      }
      catch
      {
        return null;
      }
    }
    public static T MyTryConvert<T>(this object value)
    {
      if (value == null)
        return default(T);
      var targetType = typeof(T);
      if (targetType.IsIEnumerable())
      {
        var valueType = value.GetType();
        if (!valueType.IsIEnumerable())
          return default(T);
        var targetElementType = targetType.GetIElementType();
        var valueElementType = valueType.GetIElementType();
        var listType = typeof(List<>);
        var constructedListType = listType.MakeGenericType(targetElementType);
        var instance = (IList)Activator.CreateInstance(constructedListType);
        foreach (var item in (IEnumerable)value)
        {
          if (targetElementType == valueElementType)
          {
            instance.Add(item);
          }
          else
          {
            var elementConvert = item.MyTryConvert(targetElementType);
            if (elementConvert != null)
              instance.Add(elementConvert);
          }
        }
        if (!targetType.IsArray)
          return (T)instance;
        var array = new object[instance.Count];
        instance.CopyTo(array, 0);
        return (T)((object)array);

      }
      var resultConvert = value.MyTryConvert(targetType);
      if (resultConvert == null)
        return default(T);
      return (T)resultConvert;
    }
    public static object MyTryConvertIEnumable(this object value, Type type)
    {
      if (value == null)
        return null;
      var valueType = value.GetType();
      var elementType = type.GetIElementType();
      var valueElementType = valueType.GetIElementType();
      if (elementType != null)
      {
        var listType = typeof(List<>);
        var constructedListType = listType.MakeGenericType(elementType);
        var instance = (IList)Activator.CreateInstance(constructedListType);
        if (valueElementType != null)
        {
          foreach (var v in ((IEnumerable)value))
          {
            var convertV = v.MyTryConvert(elementType);
            if (convertV == null)
              continue;
            instance.Add(convertV);
          }
        }
        if (type.IsArray)
        {
          if (instance.Count == 0)
            return null;
          Array array = new object[instance.Count];
          instance.CopyTo(array, 0);
          return array;
        }
        return instance;
      }
      return null;
    }
  }
}
