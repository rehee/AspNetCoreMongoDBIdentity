using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MUser : IdentityUser
  {

  }
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MRole : IdentityRole<string>
  {

  }
  [BsonIgnoreExtraElements(ignoreExtraElements: true, Inherited = true)]
  public class MUserRole : IdentityUserRole<string>
  {

  }
  public class MRoleStore<T> : IRoleStore<T> where T : class
  {
    public Task<IdentityResult> CreateAsync(T role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(T role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {

    }

    public Task<T> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<T> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetNormalizedRoleNameAsync(T role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetRoleIdAsync(T role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetRoleNameAsync(T role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetNormalizedRoleNameAsync(T role, string normalizedName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetRoleNameAsync(T role, string roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(T role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }

  public class MUserRoleStore<TUser, TRole, TUserRole> : IUserRoleStore<TUser> where TUser : IdentityUser where TRole : IdentityRole<string> where TUserRole : IdentityUserRole<string>
  {
    private IMongoDatabase db { get; set; }
    private IRoleStore<TRole> roles { get; set; }
    private string mRoleName { get; set; }
    private string mUserRoleName { get; set; }
    public MUserRoleStore(IMongoDatabase db, IRoleStore<TRole> roles)
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
        var collection = db.GetCollection<BsonDocument>(mUserRoleName);
        var a = new MUserRole();
        var roles = collection.Find(new { UserId = thisUser.Id }.ToBsonDocument()).ToList().Select(b => BsonSerializer.Deserialize<MUserRole>(b)).Select(b => ObjectId.Parse(b.RoleId)).ToList();
        if (roles.Count == 0)
        {
          return new List<string>();
        }
        var roleCollection = db.GetCollection<BsonDocument>(mRoleName);
        var filter = Builders<BsonDocument>.Filter.In("_id", roles);
        var roleName = roleCollection.Find(filter).ToList().Select(b=>BsonSerializer.Deserialize<MRole>(b)).ToList();
        return roleName.Select(b=>b.Name).ToList();
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
  public class MUserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserLoginStore<TUser>, IUserRoleStore<TUser> where TUser : IdentityUser
  {
    private IMongoDatabase db { get; set; }
    private IUserRoleStore<TUser> userRole { get; set; }
    private string mUserName { get; set; }
    public MUserStore(IMongoDatabase db, IUserRoleStore<TUser> userRole)
    {
      this.db = db;
      this.userRole = userRole;
      this.mUserName = typeof(TUser).Name;
    }
    public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
      Task<IdentityResult> t = new Task<IdentityResult>((u) =>
      {
        try
        {
          var d = user.ToBsonDocument();
          var type = u.GetType();
          var collection = db.GetCollection<BsonDocument>(type.Name);
          collection.InsertOne(d);
          return IdentityResult.Success;
        }
        catch (Exception ex)
        {
          var a = new IdentityError();
          a.Code = "500";
          a.Description = ex.Message;
          return IdentityResult.Failed(a);
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
        var a = this.db.GetCollection<BsonDocument>(mUserName).Find(new { Id = ObjectId.Parse((string)id) }.ToBsonDocument()).FirstOrDefault();
        if (a == null)
        {
          return default(TUser);
        }
        return BsonSerializer.Deserialize<TUser>(a);
      }, userId);

      u.Start();
      return u;
    }

    public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
      var user = new Task<TUser>((name) =>
      {
        var a = this.db.GetCollection<BsonDocument>(mUserName).Find(new { NormalizedUserName = name }.ToBsonDocument()).FirstOrDefault();
        if (a == null)
        {
          return default(TUser);
        }
        else
        {
          return BsonSerializer.Deserialize<TUser>(a);
        }
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
