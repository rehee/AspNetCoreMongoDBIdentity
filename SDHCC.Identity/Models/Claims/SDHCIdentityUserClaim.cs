using SDHCC.Identity.Models.ClaimS;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Models.Claims
{
  public class SDHCIdentityUserClaim: SDHCClaim
  {
    public string UserId { get; set; }
  }
}
