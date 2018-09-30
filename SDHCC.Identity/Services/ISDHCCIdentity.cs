using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SDHCC.Identity.Services
{
  public interface ISDHCCIdentity
  {
    bool IsUserInRole(ClaimsPrincipal user, string role);
    bool IsUserInRole(string userName, string role);
  }
}
