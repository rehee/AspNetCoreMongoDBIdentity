using MongoDB.Bson;
using MongoDB.Driver;
using SDHCC.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC.DB
{
  public class SearchParam
  {
    public object SearchObject { get; set; }
    public string EntityName { get; set; }
    public string FullName { get; set; }
    public string ReturnEntity { get; set; }
    public string ReturnFullName { get; set; }
  }

  public class FilterParam
  {
    public string EntityName { get; set; } = "";
    public string FullName { get; set; } = "";
    public List<SearchFilter> Filters { get; set; } = new List<SearchFilter>();
  }
  public class SearchFilter
  {
    public CompareOption Compare { get; set; }
    public string Property { get; set; }
    public dynamic Value { get; set; }
    public dynamic Value2 { get; set; }
  }

}

namespace System
{
  public static class SCHCCDBExtend
  {
    public static FilterDefinition<T> GetFilter<T>(this SearchFilter filter)
    {
      switch (filter.Compare)
      {
        case CompareOption.Eq:
          var obj = filter.Value as object;
          return (FilterDefinition<T>)obj.ToBsonDocument();
        case CompareOption.In:
          return Builders<T>.Filter.In(filter.Property, filter.Value);
        case CompareOption.Lt:
          return Builders<T>.Filter.Lt(filter.Property, filter.Value);
        case CompareOption.Lte:
          return Builders<T>.Filter.Lte(filter.Property, filter.Value);
        case CompareOption.Mod:
          return Builders<T>.Filter.Mod(filter.Property, filter.Value, filter.Value2);
        case CompareOption.Ne:
          return Builders<T>.Filter.Ne(filter.Property, filter.Value);
        case CompareOption.Near:
          return Builders<T>.Filter.Near(filter.Property, filter.Value);
        case CompareOption.NearSphere:
          return Builders<T>.Filter.NearSphere(filter.Property, filter.Value);
        case CompareOption.Nin:
          return Builders<T>.Filter.Nin(filter.Property, filter.Value);
        case CompareOption.Not:
          return Builders<T>.Filter.Not(
            SCHCCDBExtend.GetFilter<T>(
              new SearchFilter() { Compare = filter.Value, Property = filter.Property, Value = filter.Value2 }
              )
            );
        case CompareOption.OfType:
          return Builders<T>.Filter.OfType(filter.Property, filter.Value);
        case CompareOption.Or:
          return Builders<T>.Filter.Or(filter.Property, filter.Value);
        case CompareOption.Regex:
          return Builders<T>.Filter.Regex(filter.Property, filter.Value);
        case CompareOption.Size:
          return Builders<T>.Filter.Size(filter.Property, filter.Value);
        case CompareOption.SizeGt:
          return Builders<BsonDocument>.Filter.SizeGt(filter.Property, filter.Value);
        case CompareOption.SizeGte:
          return Builders<T>.Filter.SizeGte(filter.Property, filter.Value);
        case CompareOption.SizeLt:
          return Builders<T>.Filter.SizeLt(filter.Property, filter.Value);
        case CompareOption.SizeLte:
          return Builders<T>.Filter.SizeLte(filter.Property, filter.Value);
        case CompareOption.Text:
          return Builders<T>.Filter.Text(filter.Property, filter.Value);
        case CompareOption.Type:
          return Builders<T>.Filter.Type(filter.Property, filter.Value);
        case CompareOption.Where:
          return Builders<T>.Filter.Where(filter.Value);
      }
      return Builders<T>.Filter.Eq(filter.Property, filter.Value);
    }
    public static FilterDefinition<T> GetFilter<T>(this IEnumerable<SearchFilter> filters)
    {
      var filterList = filters.Select(b => b.GetFilter<T>()).ToList();
      var filter = filterList[0];
      for(var i = 1; i < filterList.Count; i++)
      {
        filter = filter & filterList[i];
      }
      return filter;
    }
   
  }
}
