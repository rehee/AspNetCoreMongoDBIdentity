using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization.Attributes;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Models.UserModels
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public abstract class SDHCCIdentityBase : IdentityUser<string>, BaseTypeEntity
  {
    private string fullType { get; set; }
    public string FullType
    {
      get
      {
        return string.IsNullOrEmpty(fullType) ? this.GetType().FullName : fullType;
      }
      set { fullType = value; }
    }
    private string assemblyName { get; set; }
    public virtual string AssemblyName
    {
      get
      {
        return string.IsNullOrEmpty(assemblyName) ? this.GetType().Assembly.GetName().Name : assemblyName;
      }
      set { assemblyName = value; }
    }
  }

}
