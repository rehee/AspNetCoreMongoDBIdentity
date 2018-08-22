using Microsoft.AspNetCore.Identity;
using SDHCC.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.Identity
{
  public class SDHCCUserRoleStore<TUser, TRole, TUserRole> : IUserRoleStore<TUser> where TUser : IdentityUser where TRole : IdentityRole<string> where TUserRole : IdentityUserRole<string>
  {
    private ISDHCCDbContext db { get; set; }
    private IRoleStore<TRole> roles { get; set; }
    private string mRoleName { get; set; }
    private string mUserRoleName { get; set; }
    public SDHCCUserRoleStore(ISDHCCDbContext db, IRoleStore<TRole> roles)
    {
      this.db = db;
      this.roles = roles;
      this.mRoleName = typeof(TRole).Name;
      this.mUserRoleName = typeof(TUserRole).Name;
    }
    public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {

    }

    public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<IList<string>>((u) =>
      {
        var thisUser = (TUser)u;
        var userRoles = this.db.Filter<TUserRole>(
          new FilterParam()
          {
            Filters = new List<SearchFilter>() { new SearchFilter() { Compare = CompareOption.Eq, Property = "UserId", Value = thisUser.Id } }
          },out var response).ToList().Select(b=>b.RoleId).ToList();

        if (userRoles.Count == 0)
        {
          return new List<string>();
        }
        var roleCollection = db.Filter<TRole>(new FilterParam()
        {
          Filters = new List<SearchFilter>() { new SearchFilter() { Compare = CompareOption.In, Property = "_id", Value = userRoles } }
        }, out var response2);
        return roleCollection.Select(b => b.Name).ToList();
      }, user);
      task.Start();
      return task;
    }

    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }

}
