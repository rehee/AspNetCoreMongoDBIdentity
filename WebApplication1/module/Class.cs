using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using SDHCC.DB;
using SDHCC.DB.Models;
using SDHCC.DB.Orms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System
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

  public class CCC : SDHCOrm
  {
    public CCC(ISDHCCDbContext db) : base(db)
    {

    }
  }
}
