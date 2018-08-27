using Microsoft.AspNetCore.Identity;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.Identity
{
  partial class SDHCCUserStore<TUser, TRole, TUserRole> :
    IUserSecurityStampStore<TUser>,
     IUserPhoneNumberStore<TUser>
    where TUser : IdentityUser<string>
    where TRole : IdentityRole<string>
    where TUserRole : IdentityUserRole<string>, BaseEntity, new()
  {
    public async Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
    {
      user.SecurityStamp = stamp;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        return user.SecurityStamp ?? "";
      });
      task.Start();
      return task;
    }

    public async Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
    {
      user.PhoneNumber = phoneNumber;
      await this.UpdateAsync(user, cancellationToken);
    }

    public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        return user.PhoneNumber ?? "";
      });
      task.Start();
      return task;
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
      var task = new Task<bool>(() =>
      {
        return user.PhoneNumberConfirmed;
      });
      task.Start();
      return task;
    }

    public async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
      user.PhoneNumberConfirmed = confirmed;
      await this.UpdateAsync(user, cancellationToken);
    }
  }
}
