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
    public static Type RootType { get; set; }
    public static ContentBase ConvertToContentBase(this BsonDocument b)
    {
      if (b == null)
        return null;
      var fullType = b.GetValue("FullType").ToString();
      var assemblyName = b.GetValue("AssemblyName").ToString();
      var type = Type.GetType($"{fullType},{assemblyName}");
      return (ContentBase)MongoDB.Bson.Serialization.BsonSerializer.Deserialize(b, type);
    }
    public static Expression<Func<BsonDocument, ContentBase>> bbb = b => ContentE.ConvertToContentBase(b);
  }

}
