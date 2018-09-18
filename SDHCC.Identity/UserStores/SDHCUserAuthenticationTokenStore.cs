using Microsoft.AspNetCore.Identity;
using SDHCC.DB.Models;
using SDHCC.Identity.Models.UserTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//IUserAuthenticationTokenStore<User>
namespace SDHCC.Identity
{
  public partial class SDHCCUserStore<TUser, TRole, TUserRole> :
    IUserAuthenticationTokenStore<TUser>
    where TUser : IdentityUser<string>
    where TRole : IdentityRole<string>
    where TUserRole : IdentityUserRole<string>, BaseEntity, new()
  {
    public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        var tokens = db.Where<SDHCIdentityUserToken>(
          b => b.UserId == user.Id && b.LoginProvider == loginProvider && b.Name == name
          ).FirstOrDefault();
        if (tokens == null)
        {
          return "";
        }
        return tokens.Value;
      });
      task.Start();
      return task;
    }

    public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
    {
      var task = new Task(() =>
      {
        var tokens = db.Where<SDHCIdentityUserToken>(
          b => b.UserId == user.Id && b.LoginProvider == loginProvider && b.Name == name
          ).ToList().Select(b => new UpdateEntity<SDHCIdentityUserToken>() { Object = b, Key = b.Id }).ToList();
        db.Remove<SDHCIdentityUserToken>(tokens);
      });
      task.Start();
      return task;
    }

    public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
    {
      var task = new Task(() =>
      {
        var tokens = db.Where<SDHCIdentityUserToken>(
          b => b.UserId == user.Id && b.LoginProvider == loginProvider && b.Name == name
          ).ToList();
        if (tokens.Count < 0)
        {
          var token = new SDHCIdentityUserToken()
          {
            Id = Guid.NewGuid().ToString(),
            FullType = typeof(SDHCIdentityUserToken).FullName,
            LoginProvider = loginProvider,
            Name = name,
            UserId = user.Id,
            Value = value,
          };
          db.Add<SDHCIdentityUserToken>(token, out var r);
          return;
        }
        if (tokens.Count > 0)
        {
          var current = tokens[0];
          current.Value = value;
          db.Update<SDHCIdentityUserToken>(current, current.Id, out var response);
        }
        if (tokens.Count > 1)
        {
          var more = tokens.Where(b => tokens.IndexOf(b) >= 1).Select(b => new UpdateEntity<SDHCIdentityUserToken>() { Object = b, Key = b.Id }).ToList();
          db.Remove<SDHCIdentityUserToken>(more);
        }
      });
      task.Start();
      return task;
    }
  }
}
