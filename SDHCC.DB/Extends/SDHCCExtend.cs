using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class SDHCCExtend
  {
    public static T GetTypeValue<T>(this BsonDocument input, string key)
    {
      if (!input.Contains(key))
      {
        return default(T);
      }
      var value = input[key];
      try
      {
        return (T)Convert.ChangeType(value, typeof(T));
      }
      catch { }
      return default(T);
    }

    public static BsonValue GetValueByKey(this BsonDocument input, string key)
    {
      if (input.Contains(key))
        return input.GetValue(key);
      var selectedKey = input.Names.Where(b => b.Equals(key, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
      if (string.IsNullOrEmpty(selectedKey))
        return null;
      return input[selectedKey];

    }

    public static string GetMongoCollectionName(this object input, out Type type)
    {
      type = null;
      if (input == null)
        return null;
      type = input.GetType();
      return type.Name;
    }
    public static string GetMongoCollectionName(this Type input)
    {
      return input.Name;
    }
    public static string GetMongoEntityId(this Type type, object input)
    {
      foreach (var p in type.GetProperties())
      {
        if (p.Name != "Id")
          continue;
        return p.GetValue(input).MyTryConvert<string>();
      }
      return null;
    }

    public static void GenerateMongoEntityId(this Type type, object input)
    {
      foreach (var p in type.GetProperties())
      {
        if (p.Name != "Id")
          continue;
        var id = p.GetValue(input).MyTryConvert<string>();
        if (string.IsNullOrEmpty(id))
        {
          id = Guid.NewGuid().ToString();
        }
        else
        {
          if (!Guid.TryParse(id, out var guid))
            id = Guid.NewGuid().ToString();
        }
        p.SetValue(input, id);
        return;
      }
    }
  }
}
