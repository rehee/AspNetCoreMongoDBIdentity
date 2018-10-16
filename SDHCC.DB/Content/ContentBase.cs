using MongoDB.Bson;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SDHCC.DB.Content
{


  public abstract class ContentBase : BaseProperty
  {

  }

  public class ContentBaseModel : ContentBase
  {

  }
  public static class ContentE
  {
    public static string RootPath { get; set; }
    public static Type RootType { get; set; }
    public static Type RootDropDown { get; set; }
    public static ContentBase ConvertToContentBase(this BsonDocument b)
    {
      return b.ConvertToGeneric<ContentBase>();
    }
    public static T ConvertToGeneric<T>(this BsonDocument b)
    {
      if (b == null)
        return default(T);
      var fullType = b.GetValueByKey("FullType");
      var assemblyName = b.GetValueByKey("AssemblyName");
      Type type;
      if (fullType == null || assemblyName == null)
      {
        type = typeof(T);
      }
      else
      {
        type = Type.GetType($"{fullType},{assemblyName}");
      }
      try
      {
        var obj = MongoDB.Bson.Serialization.BsonSerializer.Deserialize(b, type);
        return (T)obj;
      }
      catch { return default(T); }

    }
    public static Expression<Func<BsonDocument, ContentBase>> bbb = b => ContentE.ConvertToContentBase(b);
  }

}
