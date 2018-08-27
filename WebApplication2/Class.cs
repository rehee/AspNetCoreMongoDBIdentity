using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MUser : IdentityUser, BaseEntity
  {
    public string FullType
    {
      get
      {
        return this.GetType().FullName;
      }
      set { }
    }
  }
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MRole : IdentityRole<string>, BaseEntity
  {
    public string FullType
    {
      get
      {
        return this.GetType().FullName;
      }
      set { }
    }
  }
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MUserRole : IdentityUserRole<string>, BaseEntity
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
  }
}
