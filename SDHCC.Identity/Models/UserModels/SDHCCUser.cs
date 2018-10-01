using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Models.UserModels
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class SDHCCUser : IdentityUser<string>, BaseEntity
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
}
