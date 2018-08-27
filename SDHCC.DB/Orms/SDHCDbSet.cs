using SDHCC.Core.MethodResponse;
using SDHCC.DB.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDHCC.DB.Orms
{
  public class SDHCIDbSet<T> where T : class, BaseEntity
  {
    private ISDHCCDbContext db { get; set; }
    public SDHCIDbSet(ISDHCCDbContext db)
    {
      this.db = db;
    }
    public void Add(T entities, out MethodResponse response)
    {
      if (entities.Id == null)
        entities.Id = Guid.NewGuid().ToString();
      db.Add<T>(entities, out response);
    }
    public void AddRange(IEnumerable<T> entities, out MethodResponse response)
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
    public void AddRange(T[] entities, out MethodResponse response)
    {
      this.AddRange((IEnumerable<T>)entities, out response);
    }
    public Task AddAsync(T entities)
    {
      var task = new Task(() =>
      {
        this.Add(entities, out var response);
      });
      task.Start();
      return task;
    }
    public Task AddRangeAsync(IEnumerable<T> entities)
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
    public Task AddRangeAsync(T[] entities)
    {
      return AddRangeAsync((IEnumerable<T>)entities);
    }
    public void AttachRange(T[] entities, out MethodResponse response)
    {
      this.AddRange(entities, out response);
    }
    public void AttachRange(IEnumerable<T> entities, out MethodResponse response)
    {
      this.AddRange(entities, out response);
    }
    public T Find(string keyValues)
    {
      return db.Find<T>(keyValues, out var response);
    }
    public Task<T> FindAsync(string keyValues)
    {
      var task = new Task<T>(() =>
      {
        return this.Find(keyValues);
      });
      task.Start();
      return task;
    }
    public void Remove(T entities)
    {
      db.Remove<T>(entities, entities.Id);
    }
    public void RemoveRange(IEnumerable<T> entities)
    {
      db.Remove<T>(
        entities.Select(b => new UpdateEntity<T>() { Object = b, Key = b.Id })
        );
    }
    public void RemoveRange(T[] entities)
    {
      this.RemoveRange((IEnumerable<T>)entities);
    }
    public void Update(T entities)
    {
      db.Update<T>(entities, entities.Id, out var response);
    }
    public void UpdateRange(IEnumerable<T> entities)
    {
      foreach(var item in entities)
      {
        this.Update(item);
      }
    }
    public void UpdateRange(T[] entities)
    {
      this.UpdateRange((IEnumerable<T>)entities);
    }
    public IQueryable<T> Where(Expression<Func<T, bool>> where = null)
    {
      return db.Where<T>(where);
    }
  }
}
