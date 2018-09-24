using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
  public static class SDHCCExtend
  {
    public static T GetTypeValue<T>(this BsonDocument input, string key)
    {
      if (!input.Contains(key))
      {
        return default(T);
      }
      var value = input[key];
      try
      {
        return (T)Convert.ChangeType(value, typeof(T));
      }
      catch { }
      return default(T);
    }

    public static BsonValue GetValueByKey(this BsonDocument input, string key)
    {
      if (input.Contains(key))
        return input.GetValue(key);
      var selectedKey = input.Names.Where(b => b.Equals(key, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
      if (string.IsNullOrEmpty(selectedKey))
        return null;
      return input[selectedKey];

    }
  }
}
