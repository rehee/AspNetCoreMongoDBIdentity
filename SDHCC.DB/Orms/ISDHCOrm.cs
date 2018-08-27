using SDHCC.Core.MethodResponse;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SDHCC.DB.Orms
{
  public interface ISDHCOrm
  {
    void Add<T>(T entities, out MethodResponse response) where T : class, BaseEntity;
    void AddRange<T>(IEnumerable<T> entities, out MethodResponse response) where T : class, BaseEntity;
    void AddRange<T>(T[] entities, out MethodResponse response) where T : class, BaseEntity;
    Task AddAsync<T>(T entities) where T : class, BaseEntity;
    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, BaseEntity;
    Task AddRangeAsync<T>(T[] entities) where T : class, BaseEntity;
    void AttachRange<T>(T[] entities, out MethodResponse response) where T : class, BaseEntity;
    void AttachRange<T>(IEnumerable<T> entities, out MethodResponse response) where T : class, BaseEntity;
    T Find<T>(string keyValues) where T : class, BaseEntity;
    Task<T> FindAsync<T>(string keyValues) where T : class, BaseEntity;
    void Remove<T>(T entities) where T : class, BaseEntity;
    void RemoveRange<T>(IEnumerable<T> entities) where T : class, BaseEntity;
    void RemoveRange<T>(T[] entities) where T : class, BaseEntity;
    void Update<T>(T entities) where T : class, BaseEntity;
    void UpdateRange<T>(IEnumerable<T> entities) where T : class, BaseEntity;
    void UpdateRange<T>(T[] entities) where T : class, BaseEntity;
    IQueryable<T> Where<T>(Expression<Func<T, bool>> where = null) where T : class, BaseEntity;
  }
}
