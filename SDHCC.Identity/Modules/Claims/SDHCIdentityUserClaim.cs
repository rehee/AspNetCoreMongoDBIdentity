using SDHCC.Identity.Modules.ClaimS;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Modules.Claims
{
  public class SDHCIdentityUserClaim: SDHCClaim
  {
    public string UserId { get; set; }
  }
}
