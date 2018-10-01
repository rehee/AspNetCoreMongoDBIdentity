﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
      var setting = configuration.GetSection("DefaultUserSetting").Get<DefaultUserSetting>();
      if (isInit)
      {
        isInit = false;
        initUserAndRole(setting);
      }
        
    }
    #region
    private void initUserAndRole(DefaultUserSetting setting)
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
    public IQueryable<UserRoleView> GetUserRoles(Expression<Func<IdentityUser<string>, bool>> where = null)
    {
      var query = userManager.Users;
      if (where != null)
      {
        query = query.Where(where).Select(b => (TUser)b);
      }

      return query.Select(b => new UserRoleView()
      {
        Id = b.Id,
        Name = b.UserName,
        Roles = userManager.GetRolesAsync(b).GetAsyncValue()
      });
    }
    public IQueryable<IdentityRole> GetRoles()
    {
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
  }
}
