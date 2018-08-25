using SDHCC.Core.MethodResponse;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SDHCC.DB
{
  public interface ISDHCCDbContext
  {
    void Add<T>(T input, out MethodResponse response) where T : BaseEntity;
    void Add(Object input, out MethodResponse response);
    void AddRange<T>(IEnumerable<T> input, out MethodResponse response) where T : BaseEntity;
    void AddRange(IEnumerable<Object> input, out MethodResponse response);
    T Find<T>(string key, out MethodResponse response) where T : BaseEntity;
    IEnumerable<T> Find<T>(IEnumerable<string> keys, out MethodResponse response) where T : BaseEntity;
    object Find(string key, string entityName, string fullName, out MethodResponse response);
    T Find<T>(object search, out MethodResponse response) where T : BaseEntity;
    object Find(SearchParam search, out MethodResponse response);
    IEnumerable<T> Filter<T>(FilterParam param, out MethodResponse response) where T : BaseEntity;
    IQueryable<T> Where<T>(Expression<Func<T, bool>> where = null) where T : BaseEntity;
    void Update<T>(T input, out MethodResponse response) where T : BaseEntity;
    void Remove<T>(T input) where T : BaseEntity;
    void Remove<T>(IEnumerable<T> input) where T : BaseEntity;
  }
}
