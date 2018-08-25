using Microsoft.AspNetCore.Identity;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SDHCC.Identity.Modules.ClaimS
{
  public class SDHCIdentityRoleClaim : SDHCClaim
  {
    public string RoleId { get; set; }
  }
}
