using Microsoft.AspNetCore.Identity;
using SDHCC.DB;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.Identity
{
  public partial class SDHCCUserStore<TUser, TRole, TUserRole> :
    IUserRoleStore<TUser>

    where TUser : IdentityUser<string>
    where TRole : IdentityRole<string>
    where TUserRole : IdentityUserRole<string>, BaseEntity, new()
  {
    public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      var roleExist = await this.IsInRoleAsync(user, roleName, cancellationToken);
      if (roleExist)
        return;
      var role = db.Where<TRole>(b => b.NormalizedName == roleName.ToLower()).FirstOrDefault();
      if (role == null)
        return;
      var userRole = new TUserRole();
      userRole.RoleId = role.Id;
      userRole.UserId = user.Id;
      userRole.Id = Guid.NewGuid().ToString();
      db.Add<TUserRole>(userRole, out var response);
    }

    public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      var roleExist = await this.IsInRoleAsync(user, roleName, cancellationToken);
      if (!roleExist)
        return;
      var role = db.Where<TRole>(b => b.NormalizedName == roleName.ToLower()).FirstOrDefault();
      if (role == null)
        return;
      var thisUserRole = db.Where<TUserRole>(b => b.UserId == user.Id && b.RoleId == role.Id).FirstOrDefault();
      if (thisUserRole == null)
        return;
      db.Remove<TUserRole>(thisUserRole, thisUserRole.Id);

    }

    public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<IList<string>>(() =>
      {
        var roleIds = this.db.Where<TUserRole>(b => b.UserId == user.Id).Select(b => (object)b.RoleId).ToList();
        var roles = this.db.Where<TRole>(b => roleIds.Contains(b.Id)).Select(b => b.NormalizedName).ToList();
        return roles;
      });
      task.Start();
      return task;
    }

    public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      var task = new Task<bool>(() =>
      {
        var roleNameLow = roleName.ToLower();
        var role = db.Where<TRole>(b => b.NormalizedName == roleNameLow).FirstOrDefault();
        if (role == null)
          return false;
        return db.Where<TUserRole>(b => b.UserId == user.Id && b.RoleId == role.Id).Count() > 0;
      });
      task.Start();
      return task;
    }

    public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

  }
}
