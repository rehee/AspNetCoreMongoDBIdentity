using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SDHCC.Identity.Services
{
  public class SDHCCIdentity<TUser> : ISDHCCIdentity where TUser : IdentityUser<string>
  {
    private UserManager<TUser> userManager;
    public SDHCCIdentity(UserManager<TUser> user)
    {
      userManager = user;
    }
    public bool IsUserInRole(ClaimsPrincipal user, string role)
    {
      if (user == null)
        return false;
      if (user.Identity == null)
        return false;
      return IsUserInRole(user.Identity.Name, role);
    }
    public bool IsUserInRole(string userName, string role)
    {
      if (string.IsNullOrEmpty(userName))
        return false;
      var user = userManager.FindByNameAsync(userName).GetAsyncValue();
      if (user == null)
        return false;
      var result = userManager.IsInRoleAsync(user, role).GetAsyncValue();
      return result;
    }

  }
}
