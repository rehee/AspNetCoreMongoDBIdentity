using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Modules
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class SDHCCBaseEntity : BaseEntity
  {
    public string Id { get; set; }
    public string FullType
    {
      get
      {
        return this.GetType().FullName;
      }
      set { }
    }
    [BsonIgnore]
    public static Func<IMongoDatabase> db { get; set; }

  }
  public interface BaseEntity
  {
    string Id { get; set; }
    string FullType { get; set; }
  }
}
