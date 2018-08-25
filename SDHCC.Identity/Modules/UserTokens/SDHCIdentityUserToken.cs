using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Modules.UserTokens
{
  class SDHCIdentityUserToken : SDHCCBaseEntity
  {
    public string UserId { get; set; }
    public string LoginProvider { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
  }
}
