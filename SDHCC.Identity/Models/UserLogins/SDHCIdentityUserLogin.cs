using Microsoft.AspNetCore.Identity;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Models.UserLogins
{
  public class SDHCIdentityUserLogin : IdentityUserLogin<string>, BaseEntity
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
