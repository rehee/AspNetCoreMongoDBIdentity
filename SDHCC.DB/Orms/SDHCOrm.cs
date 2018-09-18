using MongoDB.Driver;
using SDHCC.Core.MethodResponse;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDHCC.DB.Orms
{
  public abstract class SDHCOrm: ISDHCOrm
  {
    private ISDHCCDbContext db { get; set; }
    public SDHCOrm(ISDHCCDbContext db)
    {
      this.db = db;
    }

    public virtual void Add<T>(T entities, out MethodResponse response) where T : class, BaseEntity
    {
      if (entities.Id == null)
        entities.Id = Guid.NewGuid().ToString();
      db.Add<T>(entities, out response);
    }
    public virtual void AddRange<T>(IEnumerable<T> entities, out MethodResponse response) where T : class, BaseEntity
    {
      var errors = new List<MethodResponse>();
      foreach (var item in entities)
      {
        this.Add(item, out var res);
        if (res.Success == false)
        {
          errors.Add(res);
        }
      }
      response = new MethodResponse();
      if (errors.Count > 0)
      {
        response.ResponseMessage = errors.FirstOrDefault().ResponseMessage;
        response.ResponseObject = errors;
      }
      else
      {
        response.Success = true;
      }
    }
    public virtual void AddRange<T>(T[] entities, out MethodResponse response) where T : class, BaseEntity
    {
      this.AddRange((IEnumerable<T>)entities, out response);
    }
    public virtual Task AddAsync<T>(T entities) where T : class, BaseEntity
    {
      var task = new Task(() =>
      {
        this.Add(entities, out var response);
      });
      task.Start();
      return task;
    }
    public virtual Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, BaseEntity
    {
      var task = new Task(() =>
      {
        foreach (var item in entities)
        {
          this.Add(item, out var response);
        }
      });
      task.Start();
      return task;
    }
    public virtual Task AddRangeAsync<T>(T[] entities) where T : class, BaseEntity
    {
      return AddRangeAsync<T>((IEnumerable<T>)entities);
    }
    public virtual void AttachRange<T>(T[] entities, out MethodResponse response) where T : class, BaseEntity
    {
      this.AddRange<T>(entities, out response);
    }
    public virtual void AttachRange<T>(IEnumerable<T> entities, out MethodResponse response) where T : class, BaseEntity
    {
      this.AddRange<T>(entities, out response);
    }
    public virtual T Find<T>(string keyValues) where T : class, BaseEntity
    {
      return db.Find<T>(keyValues, out var response);
    }
    public virtual Task<T> FindAsync<T>(string keyValues) where T : class, BaseEntity
    {
      var task = new Task<T>(() =>
      {
        return this.Find<T>(keyValues);
      });
      task.Start();
      return task;
    }
    public virtual void Remove<T>(T entities) where T : class, BaseEntity
    {
      db.Remove<T>(entities, entities.Id);
    }
    public virtual void RemoveRange<T>(IEnumerable<T> entities) where T : class, BaseEntity
    {
      db.Remove<T>(
        entities.Select(b => new UpdateEntity<T>() { Object = b, Key = b.Id })
        );
    }
    public virtual void RemoveRange<T>(T[] entities) where T : class, BaseEntity
    {
      this.RemoveRange<T>((IEnumerable<T>)entities);
    }
    public virtual void Update<T>(T entities) where T : class, BaseEntity
    {
      db.Update<T>(entities, entities.Id, out var response);
    }
    public virtual void UpdateRange<T>(IEnumerable<T> entities) where T : class, BaseEntity
    {
      foreach (var item in entities)
      {
        this.Update<T>(item);
      }
    }
    public virtual void UpdateRange<T>(T[] entities) where T : class, BaseEntity
    {
      this.UpdateRange<T>((IEnumerable<T>)entities);
    }
    public virtual IQueryable<T> Where<T>(Expression<Func<T, bool>> where = null) where T : class, BaseEntity
    {
      return db.Where<T>(where);
    }
  }
}
