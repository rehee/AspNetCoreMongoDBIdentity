﻿using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Modules.UserRoles
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class SDHCUserRole : IdentityUserRole<string>, BaseEntity
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
