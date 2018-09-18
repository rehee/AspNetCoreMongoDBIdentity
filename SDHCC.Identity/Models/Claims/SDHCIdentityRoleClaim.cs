using Microsoft.AspNetCore.Identity;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SDHCC.Identity.Models.ClaimS
{
  public class SDHCIdentityRoleClaim : SDHCClaim
  {
    public string RoleId { get; set; }
  }
}
