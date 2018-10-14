using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
  public static class ReflectAttribute
  {
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
    public static C CastToType<C>(this object input)
    {
      //Type genericClassType = typeof(ReflectAttribute);
      //MethodInfo methodInfo = genericClassType.GetMethod("CastToType", BindingFlags.Public | BindingFlags.Static);
      //Type[] genericArguments = new Type[] { elementType };
      //MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);
      //var final = types.Select(b => genericMethodInfo.Invoke(null, new object[] { b })).ToArray();
      return (C)input;
    }
    public static void SetValues(this PropertyInfo p, object obj, object[] types)
    {
      var elementType = p.PropertyType.GetElementType().FullName;

      if (elementType == typeof(Type).FullName)
      {
        p.SetValue(obj, types.Select(b => b as Type).ToArray());
        return;
      }
      if (elementType == typeof(string).FullName)
      {
        p.SetValue(obj, types.Select(b => b as string).ToArray());
        return;
      }
    }
    public static T GetObjectCustomAttribute<T>(this Object o, bool getField = true) where T : Attribute, new()
    {
      var cAttributes = o.GetObjectCustomAttribute(getField);
      var customeAttribute = cAttributes
        .Where(b => b.AttributeType == typeof(T)).FirstOrDefault();
      if (customeAttribute == null)
        return default(T);

      var attribute = new T();
      foreach (var p in attribute.GetType().GetProperties())
      {

        var value = customeAttribute.NamedArguments.Where(b => b.MemberName == p.Name).FirstOrDefault();
        if (value == null || value.TypedValue.Value == null)
        {
          continue;
        }
        if (p.PropertyType.IsArray)
        {
          var types = ((System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument>)value.TypedValue.Value)
            .Select(b => b.Value).ToArray();
          p.SetValues(attribute, types);
        }
        else
        {
          p.SetValue(attribute, value.TypedValue.Value);
        }
      }
      return attribute;
    }
    public static DisplayAttribute GetObjectDisplayAttribute(this Object o, bool getField = true)
    {
      return o.GetObjectCustomAttribute<DisplayAttribute>(getField);
    }
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

  }
}
