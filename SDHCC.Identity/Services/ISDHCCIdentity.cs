using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using SDHCC.Identity.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace SDHCC.Identity.Services
{
  public interface ISDHCCIdentity
  {
    bool IsUserInRole(ClaimsPrincipal user, string role);
    bool IsUserInRole(string userName, string role);
    bool IsUserInRoles(ClaimsPrincipal user, IEnumerable<string> roles, bool isBackSite = true);
    bool IsUserInRoles(ClaimsPrincipal user, BsonArray roles, bool isBackSite = true);
    IQueryable<UserRoleView> GetUserRoles(Expression<Func<IdentityUser<string>, bool>> where = null);
    IQueryable<IdentityRole> GetRoles();
    void AddRole(string roleName, out IdentityRole result);
    string CheckPassword(string login, string password);
    void SignOut();
  }
}
