using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Models
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public abstract class SDHCCBaseEntity : BaseEntity
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
    public virtual string AssemblyName
    {
      get
      {
        return "SDHCC.DB";
      }
      set { }
    }
    [BsonIgnore]
    public static Func<IMongoDatabase> db { get; set; }

    [BsonIgnore]
    public static ISDHCCDbContext context { get; set; }

    public void GenerateId()
    {
      this.Id = Guid.NewGuid().ToString();
    }
  }
  public interface BaseEntity
  {
    string Id { get; set; }
    string FullType { get; set; }
  }
}
