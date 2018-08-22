using Microsoft.AspNetCore.Identity;
using SDHCC.DB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.Identity
{
  public class SDHCCUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserLoginStore<TUser>, IUserRoleStore<TUser> where TUser : IdentityUser
  {
    private ISDHCCDbContext db { get; set; }
    private IUserRoleStore<TUser> userRole { get; set; }
    public SDHCCUserStore(ISDHCCDbContext db, IUserRoleStore<TUser> userRole)
    {
      this.db = db;
      this.userRole = userRole;
    }
    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
      Task<IdentityResult> t = new Task<IdentityResult>((u) =>
      {
        try
        {
          db.Add(user, out var response);
          if (response.Success != true)
          {
            var f = new IdentityError();
            f.Code = "500";
            f.Description = response.ResponseMessage;
            return IdentityResult.Failed(f);
          }
          return IdentityResult.Success;
        }
        catch (Exception ex)
        {
          var f = new IdentityError();
          f.Code = "500";
          f.Description = ex.Message;
          return IdentityResult.Failed(f);
        }
      }, user);
      t.Start();
      return t;
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
      var u = new Task<TUser>((id) =>
      {
        var userFind = this.db.Find<TUser>(userId, out var response);
        return userFind;
      }, userId);

      u.Start();
      return u;
    }

    public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
      var user = new Task<TUser>((name) =>
      {
        var userFind = this.db.Find<TUser>(new { NormalizedUserName = name }, out var response);
        return userFind;
      }, normalizedUserName.Trim().ToLower());
      user.Start();
      return user;
    }

    public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
      var t = new Task<string>((u) => { return ((TUser)u).Id; }, user);
      t.Start();
      return t;
    }

    public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
      var t = new Task<string>(() =>
      {
        if (!String.IsNullOrEmpty(user.UserName))
        {
          return user.UserName.Trim().ToLower();
        }
        if (!String.IsNullOrEmpty(user.Email))
        {
          return user.Email.Trim().ToLower();
        }
        return user.Id.ToLower();
      });
      t.Start();
      return t;

    }


    public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
    {
      var setNormalize = new SetPasswordHash<TUser>()
      {
        user = user,
        passwordHash = normalizedName,
      };
      var t = new Task((sets) =>
      {
        var a = (SetPasswordHash<TUser>)sets;
        a.user.NormalizedUserName = a.passwordHash.ToLower();
      }, setNormalize);
      t.Start();
      return t;
    }

    public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
    class SetPasswordHash<T>
    {
      public T user { get; set; }
      public string passwordHash { get; set; }
    }
    public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
    {

      var parms = new SetPasswordHash<TUser>()
      {
        user = user,
        passwordHash = passwordHash
      };

      var setPasswordHash = new Task((p) =>
      {
        var parm = (SetPasswordHash<TUser>)p;
        parm.user.PasswordHash = passwordHash;
      }, parms);
      setPasswordHash.Start();
      return setPasswordHash;
    }

    public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
      var a = userRole.GetRolesAsync(user, cancellationToken);
      return a;
    }

    public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }

}
