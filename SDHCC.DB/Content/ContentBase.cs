using MongoDB.Bson;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SDHCC.DB.Content
{
  public abstract class ContentBase : SDHCCBaseEntity
  {
    public string ParentId { get; set; } = "";
    public List<string> Children { get; set; } = new List<string>();
    private string name { get; set; }
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

    public DateTime CreateTime { get; set; }
    public int SortOrder { get; set; } = 0;
  }

  public class ContentBaseModel : ContentBase
  {

  }
  public static class ContentE
  {
    public static Type RootType { get; set; }
    public static ContentBase ConvertToContentBase(this BsonDocument b)
    {
      return (ContentBase)MongoDB.Bson.Serialization.BsonSerializer.Deserialize(b, Type.GetType($"{b["FullType"].ToString()},{b["AssemblyName"].ToString()}"));
    }
    public static Expression<Func<BsonDocument, ContentBase>> bbb = b => ContentE.ConvertToContentBase(b);
  }

}
