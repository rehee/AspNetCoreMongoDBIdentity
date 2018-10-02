using MongoDB.Bson;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SDHCC.DB.Content
{
  public abstract class ContentBaseProperty : SDHCCBaseEntity
  {
    [BaseProperty]
    public string ParentId { get; set; } = "";
    [BaseProperty]
    public List<string> Children { get; set; } = new List<string>();
    private string name { get; set; }
    [BaseProperty]
    public string Name
    {
      get
      {
        return name;
      }
      set
      {
        name = String.IsNullOrEmpty(value) ? this.Id : value.Trim().Replace('/', '_').ToLower();
      }
    }
    [BaseProperty]
    public DateTime CreateTime { get; set; }
    [BaseProperty]
    public int SortOrder { get; set; } = 0;
  }
  public abstract class ContentBase : ContentBaseProperty
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
