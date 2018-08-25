using Microsoft.AspNetCore.Identity;
using SDHCC.DB;
using SDHCC.DB.Modules;
using SDHCC.Identity.Modules;
using SDHCC.Identity.Modules.ClaimS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.Identity
{
  public class SDHCCRoleStore<TRole, TUserRole> :
    IRoleStore<TRole>,
    IQueryableRoleStore<TRole>,
    IRoleClaimStore<TRole>
    where TRole : IdentityRole<string>, BaseEntity
    where TUserRole : IdentityUserRole<string>, BaseEntity
  {
    private ISDHCCDbContext db { get; set; }
    public SDHCCRoleStore(ISDHCCDbContext db)
    {
      this.db = db;
    }
    public IQueryable<TRole> Roles => db.Where<TRole>();

    public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {
      var task = new Task<IdentityResult>(() =>
      {
        db.Add<TRole>(role, out var response);
        if (response.Success)
          return IdentityResult.Success;
        return IdentityResult.Failed(new IdentityError[1]
        {
          new IdentityError()
          {
            Code = response.ResponseCode.ToString(),
            Description = response.ResponseMessage,
          }
        });
      });
      task.Start();
      return task;
    }

    public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
      var task = new Task<IdentityResult>(() =>
      {
        var userRoles = db.Where<TUserRole>(b => b.UserId == role.Id).ToList();
        if (userRoles.Count > 0)
        {
          db.Remove<TUserRole>(userRoles);
        }
        db.Remove<TRole>(role);
        return IdentityResult.Success;
      });
      task.Start();
      return task;
    }

    public void Dispose()
    {
      
    }

    public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
      var task = new Task<TRole>(() =>
      {
        return db.Find<TRole>(roleId, out var response);
      });
      task.Start();
      return task;
    }

    public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
      var name = normalizedRoleName.Trim().ToLower();
      var task = new Task<TRole>(() =>
      {
        return db.Where<TRole>(b => b.NormalizedName == name).FirstOrDefault();
      });
      task.Start();
      return task;
    }

    public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        if (String.IsNullOrEmpty(role.NormalizedName))
        {
          return role.Name.Trim().ToLower();
        }
        return role.NormalizedName.ToLower();
      });
      task.Start();
      return task;
    }

    public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        return role.Id;
      });
      task.Start();
      return task;
    }

    public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
      var task = new Task<string>(() =>
      {
        return role.Name;
      });
      task.Start();
      return task;
    }

    public async Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
    {
      role.NormalizedName = normalizedName.Trim().ToLower();
      await this.UpdateAsync(role, cancellationToken);
    }

    public async Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
    {
      role.Name = roleName.Trim();
      await this.UpdateAsync(role, cancellationToken);
    }

    public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
      var task = new Task<IdentityResult>(() =>
      {
        db.Update<TRole>(role, out var response);
        if (response.Success)
          return IdentityResult.Success;
        return IdentityResult.Failed(new IdentityError[1]
        {
          new IdentityError()
          {
            Code = response.ResponseCode.ToString(),
            Description = response.Message,
          }
        });
      });
      task.Start();
      return task;
    }

    public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
      var task = new Task<IList<Claim>>(() =>
      {
        try
        {
          var roleClaims = db.Where<SDHCIdentityRoleClaim>(b => b.RoleId == role.Id).ToList();
          return roleClaims.Select(b => b.ToClaim()).ToList();
        }
        catch
        {
          return new List<Claim>();
        }
      });
      task.Start();
      return task;
    }
    public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
    {
      var claims = await this.GetClaimsAsync(role);
      var counts = claims.Where(b => b.Type == claim.Type && b.Value == claim.Value).Count();
      if (counts > 0)
      {
        return;
      }
      var roleClaim = new SDHCIdentityRoleClaim()
      {
        ClaimType = claim.Type,
        ClaimValue = claim.Value,
        RoleId = role.Id,
      };
      db.Add<SDHCIdentityRoleClaim>(roleClaim, out var response);
    }
    public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
    {
      var task = new Task(() =>
      {
        try
        {
          var roleClaim = db.Where<SDHCIdentityRoleClaim>(b => b.RoleId == role.Id && b.ClaimType == claim.Type && b.ClaimValue == claim.Value).ToList();
          db.Remove<SDHCIdentityRoleClaim>(roleClaim);
        }
        catch { }
      });
      task.Start();
      return task;
    }
  }

}
