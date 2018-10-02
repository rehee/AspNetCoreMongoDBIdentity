using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

namespace SDHCC.DB.Models
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public abstract class SDHCCBaseEntity : BaseTypeEntity
  {
    [BaseProperty]
    public string Id { get; set; }
    private string fullType { get; set; }
    [BaseProperty]
    public string FullType
    {
      get
      {
        return string.IsNullOrEmpty(fullType) ? this.GetType().FullName : fullType;
      }
      set { fullType = value; }
    }
    private string assemblyName { get; set; }
    [BaseProperty]
    public string AssemblyName
    {
      get
      {
        return string.IsNullOrEmpty(assemblyName) ? this.GetType().Assembly.GetName().Name : assemblyName;
      }
      set { assemblyName = value; }
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
  public interface BaseTypeEntity: BaseEntity
  {
    string AssemblyName { get; set; }
  }
  public interface BaseEntity
  {
    string Id { get; set; }
    string FullType { get; set; }
  }
}
