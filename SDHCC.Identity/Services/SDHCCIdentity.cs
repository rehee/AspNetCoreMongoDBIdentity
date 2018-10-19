using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using SDHCC.Identity.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SDHCC.Identity.Services
{
  public class SDHCCIdentity<TUser> : ISDHCCIdentity where TUser : IdentityUser<string>, new()
  {
    private UserManager<TUser> userManager;
    private RoleManager<IdentityRole> roleManager;
    private SignInManager<TUser> signInManager;
    private IConfiguration configuration;
    private static bool isInit = true;
    public SDHCCIdentity(
      UserManager<TUser> user,
      RoleManager<IdentityRole> roleManager,
      SignInManager<TUser> signInManager,
      IConfiguration configuration)
    {
      userManager = user;
      this.roleManager = roleManager;
      this.signInManager = signInManager;
      this.configuration = configuration;
      if (isInit)
      {
        isInit = false;
        initUserAndRole(E.Setting);
      }

    }
    #region
    private void initUserAndRole(SiteSetting setting)
    {
      AddRole(setting.AdminRole, out var result);
      AddRole(setting.BackUser, out result);
      var name = setting.Name.Trim().ToLower();
      var userCheck = userManager.FindByNameAsync(name).GetAsyncValue();
      if (userCheck == null)
      {
        var newUser = new TUser();
        newUser.Id = Guid.NewGuid().ToString();
        newUser.NormalizedEmail = name;
        newUser.Email = name;
        newUser.EmailConfirmed = true;
        newUser.UserName = name;
        newUser.NormalizedUserName = name;
        var userCreateResult = userManager.CreateAsync(newUser, setting.Password).GetAsyncValue();
        userManager.AddToRoleAsync(newUser, setting.AdminRole).GetAsyncValue();
        userManager.AddToRoleAsync(newUser, setting.BackUser).GetAsyncValue();
      }
      else
      {
        if (IsUserInRole(userCheck.NormalizedUserName, setting.AdminRole))
        {
          userManager.AddToRoleAsync(userCheck, setting.AdminRole).GetAsyncValue();
        }
        if (IsUserInRole(userCheck.NormalizedUserName, setting.BackUser))
        {
          userManager.AddToRoleAsync(userCheck, setting.BackUser).GetAsyncValue();
        }
      }
    }
    #endregion
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
    public bool IsUserInRoles(ClaimsPrincipal user, IEnumerable<string> roles, bool isBackSite = true)
    {
      if (user == null)
        return false;
      if (!user.Identity.IsAuthenticated)
        return false;
      var checkUser = userManager.FindByNameAsync(user.Identity.Name).GetAsyncValue();
      if (checkUser == null)
        return false;
      if (isBackSite)
      {
        var isAdmin = userManager.IsInRoleAsync(checkUser, E.Setting.AdminRole).GetAsyncValue();
        if (isAdmin)
        {
          return true;
        }
        var isBackUser = userManager.IsInRoleAsync(checkUser, E.Setting.BackUser).GetAsyncValue();
        if (!isBackUser)
        {
          return false;
        }
      }
      foreach (var role in roles)
      {
        var checkRoles = userManager.IsInRoleAsync(checkUser, role).GetAsyncValue();
        if (!checkRoles)
          return false;
      }
      return true;
    }
    public bool IsUserInRoles(ClaimsPrincipal user, BsonArray roles, bool isBackSite = true)
    {
      var list = new List<string>();
      foreach (var item in roles)
      {
        list.Add(item.ToString());
      }
      return IsUserInRoles(user, list, isBackSite);
    }
    public IQueryable<UserRoleView> GetUserRoles(Expression<Func<IdentityUser<string>, bool>> where = null)
    {
      var query = userManager.Users;
      if (where != null)
      {
        query = query.Where(where).Select(b => (TUser)b);
      }
      var user = query.ToList();
      var result = new List<UserRoleView>();
      foreach (var b in user)
      {
        var r = userManager.GetRolesAsync(b).GetAsyncValue();
        result.Add(new UserRoleView()
        {
          Id = b.Id,
          Name = b.UserName,
          Roles = r
        });
      }
      return result.AsQueryable<UserRoleView>();
    }
    public IQueryable<IdentityRole> GetRoles(bool noSystemRole = false)
    {
      if (noSystemRole)
      {
        var systemRole = new List<string>();
        if (!string.IsNullOrEmpty(E.Setting.AdminRole))
          systemRole.Add(E.Setting.AdminRole.ToLower());
        if (!string.IsNullOrEmpty(E.Setting.BackUser))
          systemRole.Add(E.Setting.BackUser.ToLower());
        return roleManager.Roles.Where(b => !systemRole.Contains(b.NormalizedName));
      }
      return roleManager.Roles;
    }
    public void AddRole(string roleName, out IdentityRole result)
    {
      if (string.IsNullOrEmpty(roleName))
      {
        result = null;
      }
      roleName = roleName.Trim().Replace(" ", "");
      var role = new IdentityRole()
      {
        Name = roleName,
      };
      var methodResponse = roleManager.CreateAsync(role).GetAsyncValue();
      if (methodResponse.Succeeded)
      {
        result = role;
      }
      else
      {
        result = null;
      }
    }
    public void AddRoles(IEnumerable<string> roleNames)
    {
      foreach(var r in roleNames)
      {
        this.AddRole(r, out var res);
      }
    }
    public void RemoveRole(string roleName)
    {
      if (string.IsNullOrEmpty(roleName))
        return;
      roleName = roleName.Trim().Replace(" ", "");
      var role = roleManager.FindByNameAsync(roleName).GetAsyncValue();
      if (role == null)
        return;
      this.roleManager.DeleteAsync(role).GetAsyncValue();
    }
    public void RemoveRoles(IEnumerable<string> roleNames)
    {
      foreach(var r in roleNames)
      {
        this.RemoveRole(r);
      }
    }
    public string CheckPassword(string login, string password)
    {
      var user = this.userManager.FindByNameAsync(login).GetAsyncValue();
      if (user == null)
      {
        user = userManager.FindByEmailAsync(login).GetAsyncValue();
      }
      if (user == null)
      {
        return null;
      }
      if (userManager.CheckPasswordAsync(user, password).GetAsyncValue())
      {
        var loginResult = this.signInManager.PasswordSignInAsync(user, password, true, true).GetAsyncValue();

        return user.NormalizedUserName;
      }
      return null;
    }
    public void SignOut()
    {
      signInManager.SignOutAsync().GetAsyncValue();
    }

    public T GetUserByName<T>(string userName) where T : IdentityUser<string>
    {
      if (String.IsNullOrEmpty(userName))
        return default(T);
      var userCheck = userManager.FindByNameAsync(userName).GetAsyncValue();
      if (userCheck == null)
      {
        return default(T);
      }
      return (T)Convert.ChangeType(userCheck, typeof(T));
    }
    public SDHCCUserBase GetUserByName(string userName)
    {
      if (String.IsNullOrEmpty(userName))
        return null;
      var userCheck = userManager.FindByNameAsync(userName).GetAsyncValue();
      if (userCheck == null)
      {
        return null;
      }
      return (SDHCCUserBase)(object)userCheck;
    }
    public SDHCCUserBase GetUserById(string userId)
    {
      if (String.IsNullOrEmpty(userId))
        return null;
      var userCheck = userManager.FindByIdAsync(userId).GetAsyncValue();
      if (userCheck == null)
      {
        return null;
      }
      return (SDHCCUserBase)(object)userCheck;
    }
    public SDHCCUserBase GetEmptyUser()
    {
      var newUser = new TUser();
      return newUser as SDHCCUserBase;
    }
    public void CreateUser(SDHCCUserBase model)
    {
      var user = model as TUser;
      this.userManager.SetEmailAsync(user, model.Email);
      this.userManager.SetUserNameAsync(user, model.UserName);
      var result = this.userManager.CreateAsync(user, model.PasswordHash).GetAsyncValue();
    }
    public void UpdateUserRole(SDHCCUserBase model, IEnumerable<string> roles)
    {
      var user = model as TUser;
      var userRoles = this.GetUserRoles(b => b.Id == model.Id).Select(b => b.Roles).FirstOrDefault().ToList();
      var deletedRoles = userRoles.Where(b => !roles.Contains(b)).ToList();
      userManager.RemoveFromRolesAsync(user, deletedRoles).GetAsyncValue();
      var addRoles = roles.Where(b => !userRoles.Contains(b)).ToList();
      userManager.AddToRolesAsync(user, addRoles).GetAsyncValue();
    }
    public void UpdateUser(SDHCCUserBase model)
    {
      var result = this.userManager.UpdateAsync(model as TUser).GetAsyncValue();
    }
  }
}
