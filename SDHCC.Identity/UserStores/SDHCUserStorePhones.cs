using Microsoft.AspNetCore.Identity;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.Identity
{
  partial class SDHCCUserStore<TUser> : 
    IUserSecurityStampStore<TUser>,
     IUserPhoneNumberStore<TUser>
    where TUser : IdentityUser, BaseEntity
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

    public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
