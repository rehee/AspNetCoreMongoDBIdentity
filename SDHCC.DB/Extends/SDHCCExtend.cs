using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SDHCC;
using System;
using System.Collections.Generic;
using System.IO;
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
        return value.MyTryConvert<T>();
      }
      catch { }
      return default(T);
    }
    public static Type GetTypeFromBson(this BsonDocument input)
    {
      if (input == null)
        return null;
      var fullType = input.GetTypeValue<string>(ContentE.FullTypeString);
      var assem = input.GetTypeValue<string>(ContentE.AssemblyNameString);
      if (string.IsNullOrEmpty(fullType) || string.IsNullOrEmpty(assem))
        return null;
      return Type.GetType($"{fullType},{assem}");
    }
    public static object ConvertBsonToObject(this BsonDocument input, Type type)
    {
      if (type == null)
        return null;
      try
      {
        return BsonSerializer.Deserialize(input, type);
      }
      catch
      {
        return null;
      }
    }
    public static object ConvertBsonToObject(this BsonDocument input)
    {
      var type = input.GetTypeFromBson();
      if (type == null)
        return null;
      try
      {
        return BsonSerializer.Deserialize(input, type);
      }
      catch
      {
        return null;
      }
    }
    public static T ConvertBsonToObject<T>(this BsonDocument input)
    {
      try
      {
        var type = input.GetTypeFromBson();
        if (type != null)
          return (T)input.ConvertBsonToObject(type);
        var valueType = typeof(T);
        if (valueType.IsInterface || valueType.IsAbstract)
          return default(T);
        return BsonSerializer.Deserialize<T>(input);
      }
      catch { return default(T); }
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
    public static void Save(this IFormFile file, out string filePath)
    {
      filePath = "";
      if (file == null)
        return;
      try
      {
        var name = file.FileName.Split('.').LastOrDefault();
        if (String.IsNullOrEmpty(name))
          name = "";
        else
          name = '.' + name;
        var path = Path.Combine(Directory.GetCurrentDirectory(),
                               ContentE.FileUploadPath, $"{Guid.NewGuid().ToString()}{name}");
        var exist = Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(),
                                 ContentE.FileUploadPath));
        if (!exist)
        {
          Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),
                                 ContentE.FileUploadPath));
        }
        using (var stream = new FileStream(path, FileMode.Create))
        {
          file.CopyToAsync(stream).GetAsyncValue();
        }
        filePath = path;
      }
      catch { }

    }
    
  }
}
