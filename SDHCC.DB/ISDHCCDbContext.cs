using SDHCC.Core.MethodResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB
{
  public interface ISDHCCDbContext
  {
    void Add(Object input,out MethodResponse response);
    void AddRange(IEnumerable<Object> input, out MethodResponse response);
    T Find<T>(string key, out MethodResponse response);
    object Find(string key, string entityName, string fullName, out MethodResponse response);
    T Find<T>(object search, out MethodResponse response);
    object Find(SearchParam search, out MethodResponse response);
    IEnumerable<T> Filter<T>(FilterParam param, out MethodResponse response);
  }
}
