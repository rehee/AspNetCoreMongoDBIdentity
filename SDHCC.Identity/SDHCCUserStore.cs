using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SDHCC.DB;
using SDHCC.DB.Modules;
using SDHCC.Identity.Modules.Claims;
using SDHCC.Identity.Modules.UserLogins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.Identity
{
  public partial class SDHCCUserStore<TUser> : IUserStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserLoginStore<TUser>,
    IUserRoleStore<TUser>,
    IUserEmailStore<TUser>,
    IUserAuthenticatorKeyStore<TUser>,
    IUserTwoFactorStore<TUser>,
    IUserTwoFactorRecoveryCodeStore<TUser>,
    IUserLockoutStore<TUser>,
    IUserClaimStore<TUser>
    where TUser : IdentityUser, BaseEntity
  {
    private ISDHCCDbContext db { get; set; }
    private IUserRoleStore<TUser> userRole { get; set; }
    public SDHCCUserStore(ISDHCCDbContext db, IUserRoleStore<TUser> userRole)
    {
      this.db = db;
      this.userRole = userRole;

    }
    public Task<TUser> GetUserAsync(ClaimsPrincipal principal)
    {
      var name = principal.Identity.Name.ToLower();
      var task = new Task<TUser>(() =>
      {
        return db.Where<TUser>(b => b.NormalizedUserName == name).FirstOrDefault();
      });
      task.Start();
      return task;
    }
    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
      Task<IdentityResult> t = new Task<IdentityResult>((u) =>
      {
        try
        {
          var users = db.Where<TUser>(b => b.UserName == user.UserName || b.NormalizedUserName == user.NormalizedUserName || b.Email == user.Email || b.EmailConfirmed == user.EmailConfirmed).Count();
          if (users > 0)
          {
            return IdentityResult.Failed(new IdentityError[1]
            {
              new IdentityError()
              {
                Code="1",
                Description="exist",
              }
            });
          }
          var insertUser = (TUser)u;
          insertUser.Email = insertUser.Email.Trim().ToLower();
          insertUser.NormalizedEmail = insertUser.NormalizedEmail.Trim().ToLower();
          db.Add<TUser>(insertUser, out var response);
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
      var task = new Task<IdentityResult>(() =>
      {
        db.Remove<TUser>(user);
        return IdentityResult.Success;
      });
      task.Start();
      return task;
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
      var task = new Task<string>(() =>
      {
        return user.NormalizedEmail;
      });
      task.Start();
      return task;
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


    public async Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
    {
      normalizedName = normalizedName.Trim().ToLower();
      var userFind = await this.FindByNameAsync(normalizedName, cancellationToken);
      if (userFind != null)
        return;
      user.NormalizedUserName = normalizedName;
      user.UserName = normalizedName;
      await this.UpdateAsync(user, cancellationToken);
    }

    public async Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
    {
      var userFind = await this.FindByNameAsync(userName.Trim(), cancellationToken);
      if (userFind != null)
        return;
      user.NormalizedUserName = userName.Trim().ToLower();
      user.UserName = userName.Trim();
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<IdentityResult>(() =>
      {
        db.Update<TUser>(user, out var response);
        if (response.Success)
        {
          return IdentityResult.Success;
        }
        return IdentityResult.Failed();
      });
      task.Start();
      return task;
    }
    public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
    {
      user.PasswordHash = passwordHash;
      return this.UpdateAsync(user, cancellationToken);
    }

    public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        return user.PasswordHash;
      });
      task.Start();
      return task;

    }

    public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<bool>(() =>
      {
        return String.IsNullOrEmpty(user.PasswordHash);
      });
      task.Start();
      return task;
    }

    public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
    {
      var task = new Task(() =>
      {
        var existLogin = db.Where<SDHCIdentityUserLogin>(
          b => b.UserId == user.Id && b.LoginProvider == login.LoginProvider && b.ProviderKey == login.ProviderKey
          ).Count();
        if (existLogin > 0)
          return;
        var sdhcLogin = new SDHCIdentityUserLogin()
        {
          UserId = user.Id,
          LoginProvider = login.LoginProvider,
          ProviderKey = login.ProviderKey,
          ProviderDisplayName = login.ProviderDisplayName,
        };
        db.Add<SDHCIdentityUserLogin>(sdhcLogin, out var response);
      });
      task.Start();
      return task;
    }

    public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
      var task = new Task(() =>
      {
        var existLogin = db.Where<SDHCIdentityUserLogin>(
          b => b.UserId == user.Id && b.LoginProvider == loginProvider && b.ProviderKey == providerKey
          ).ToList();
        if (existLogin.Count < 0)
          return;
        db.Remove<SDHCIdentityUserLogin>(existLogin);
      });
      task.Start();
      return task;
    }

    public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<IList<UserLoginInfo>>(()=> 
      {
        return db.Where<SDHCIdentityUserLogin>(b => b.UserId == user.Id)
        .Select(b => new UserLoginInfo(b.LoginProvider, b.ProviderKey, b.ProviderDisplayName))
        .ToList();
      });
      task.Start();
      return task;
    }

    public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
      var task = new Task<TUser>(() =>
      {
        var login = db.Where<SDHCIdentityUserLogin>(b => b.LoginProvider == loginProvider && b.ProviderKey == providerKey).FirstOrDefault();
        if (login == null)
        {
          return null;
        }
        return db.Find<TUser>(login.UserId, out var response);
      });
      task.Start();
      return task;
    }

    public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      return userRole.AddToRoleAsync(user, roleName, cancellationToken);
    }

    public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      return userRole.RemoveFromRoleAsync(user, roleName, cancellationToken);
    }

    public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
      return userRole.GetRolesAsync(user, cancellationToken);
    }

    public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
      return this.userRole.IsInRoleAsync(user, roleName, cancellationToken);
    }

    public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
      return this.userRole.GetUsersInRoleAsync(roleName, cancellationToken);
    }

    public async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
    {
      email = email.Trim().ToLower();
      var userFind = await this.FindByEmailAsync(email, cancellationToken);
      if (userFind != null)
        return;
      user.NormalizedEmail = email;
      user.Email = email;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        return user.Email;
      });
      task.Start();
      return task;
    }

    public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<bool>(() =>
      {
        return user.EmailConfirmed;
      });
      task.Start();
      return task;
    }

    public async Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
      user.EmailConfirmed = confirmed;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
      var task = new Task<TUser>(() =>
      {
        return db.Where<TUser>(b => b.NormalizedEmail == normalizedEmail.ToLower()).FirstOrDefault();
      });
      task.Start();
      return task;
    }

    public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        return user.NormalizedEmail;
      });
      task.Start();
      return task;
    }

    public async Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
    {
      user.NormalizedEmail = normalizedEmail.Trim().ToLower();
      await this.UpdateAsync(user, cancellationToken);
    }

    public async Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
    {
      user.ConcurrencyStamp = key;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<string>((u) =>
      {
        return ((TUser)u).ConcurrencyStamp;
      }, user);
      task.Start();
      return task;
    }

    public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
    {
      var task = new Task(async (u) =>
      {
        ((TUser)u).TwoFactorEnabled = enabled;
        await this.UpdateAsync(user, cancellationToken);
      }, user);
      task.Start();
      return task;
    }

    public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<bool>((u) =>
      {
        return ((TUser)u).TwoFactorEnabled;
      }, user);
      task.Start();
      return task;
    }

    public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
    {

      throw new NotImplementedException();
    }

    public Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<DateTimeOffset?>(() =>
      {
        return user.LockoutEnd;
      });
      task.Start();
      return task;
    }

    public async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
      user.LockoutEnd = lockoutEnd;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<int>(() =>
      {
        var result = user.AccessFailedCount + 1;
        user.AccessFailedCount = result;
        this.UpdateAsync(user, cancellationToken);
        return result;
      });
      task.Start();
      return task;
    }

    public async Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
    {
      user.AccessFailedCount = 0;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<int>(() =>
      {
        return user.AccessFailedCount;
      });
      task.Start();
      return task;
    }

    public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<bool>(() =>
      {
        return user.LockoutEnabled;
      });
      task.Start();
      return task;
    }

    public async Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
    {
      user.LockoutEnabled = enabled;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<IList<Claim>>(() =>
      {
        var records = db.Where<SDHCIdentityUserClaim>(b => b.UserId == user.Id);
        return records.Select(b => new Claim(b.ClaimType,b.ClaimValue)).ToList();
      });
      task.Start();
      return task;
    }

    public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
      var task = new Task(() =>
      {
        foreach (var item in claims)
        {
          var exist = db.Where<SDHCIdentityUserClaim>(b => b.UserId == user.Id && b.ClaimValue == item.Value && b.ClaimType == item.Type).Count();
          if (exist > 0)
            continue;
          var newClaims = new SDHCIdentityUserClaim()
          {
            UserId = user.Id,
            ClaimType = item.Type,
            ClaimValue = item.Value,
          };
          db.Add<SDHCIdentityUserClaim>(newClaims, out var response);
        }
      });
      task.Start();
      return task;
    }

    public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
      var task = new Task(() =>
      {
        var existClain = db.Where<SDHCIdentityUserClaim>(
          b => b.UserId == user.Id && b.ClaimType == claim.Type && b.ClaimValue == claim.Value
          ).ToList();
        var first = existClain.FirstOrDefault();
        first.ClaimType = newClaim.Type;
        first.ClaimValue = newClaim.Value;
        db.Update<SDHCIdentityUserClaim>(first, out var response);
        if (existClain.Count > 1)
        {
          var exture = existClain.Where(b => existClain.IndexOf(b) > 1).ToList();
          db.Remove<SDHCIdentityUserClaim>(exture);
        }
      });
      task.Start();
      return task;
    }

    public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
      var task = new Task(() =>
      {
        foreach (var item in claims)
        {
          var list = db.Where<SDHCIdentityUserClaim>(b => b.UserId == user.Id && b.ClaimValue == b.ClaimValue && b.ClaimType == item.Type).ToList();
          db.Remove<SDHCIdentityUserClaim>(list);
        }
      });
      task.Start();
      return task;
    }

    public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
      var task = new Task<IList<TUser>>(() =>
      {
        var userIds = db.Where<SDHCIdentityUserClaim>(b => b.ClaimType == claim.Type).Select(b => b.UserId).Distinct().ToList();
        return db.Where<TUser>(b => userIds.Contains(b.Id)).ToList();
      });
      task.Start();
      return task;
    }
  }

}
