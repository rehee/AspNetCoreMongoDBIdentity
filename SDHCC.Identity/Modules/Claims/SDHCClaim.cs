using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SDHCC.Identity.Modules.ClaimS
{
  public class SDHCClaim : SDHCCBaseEntity
  {
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
    public void InitializeFromClaim(Claim other)
    {
      this.ClaimType = other.Type;
      this.ClaimValue = other.Value;
    }
    public Claim ToClaim()
    {
      return new Claim(this.ClaimType, this.ClaimValue);
    }
  }
}
