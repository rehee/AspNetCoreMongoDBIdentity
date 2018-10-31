using MongoDB.Bson;
using SDHCC.Core.MethodResponse;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SDHCC.DB
{
  public partial interface ISDHCCDbContext
  {
    Expression<Func<BsonDocument, T>> ConvertBsonToGeneric<T>();
    void Add<T>(T input, out MethodResponse response) where T : class;
    void Add<T>(T input, string entityName, out MethodResponse response) where T : class;
    void Add(object input, out MethodResponse response);

    void AddRange<T>(IEnumerable<T> input, out MethodResponse response) where T : class;
    void AddRange<T>(IEnumerable<T> input, string entityName, out MethodResponse response) where T : class;
    void AddRange(IEnumerable<object> input, out MethodResponse response);

    T Find<T>(string key, string entityName, out MethodResponse response) where T : class;
    T Find<T>(string key, out MethodResponse response) where T : class;
    T Find<T>(string key, string entityName, Expression<Func<BsonDocument, T>> convert, out MethodResponse response);
    
    BsonDocument Find(string key, string entityName, out MethodResponse response);
    object Find(string key, string entityName, string fullName, out MethodResponse response);

    T Find<T>(object search, out MethodResponse response) where T : class;
    object Find(SearchParam search, out MethodResponse response);

    IEnumerable<T> Find<T>(IEnumerable<string> keys, out MethodResponse response) where T : class;
    IQueryable<T> Find<T>(IEnumerable<string> keys, string entityName, out MethodResponse response) where T : class;
    IEnumerable<object> Find(IEnumerable<string> keys, string entityName, string fullName, out MethodResponse response);
    
    //IEnumerable<T> Filter<T>(FilterParam param, out MethodResponse response) where T : class;
    void Update<T>(T input, string id, out MethodResponse response) where T : class;
    void Update<T>(T input, string id, string entityName, out MethodResponse response) where T : class;
    void Update<T>(T input, string id, string entityName, IEnumerable<string> ignoreKeys, IEnumerable<string> takeKeys, out MethodResponse response) where T : class;

    IQueryable<T> Where<T>(Expression<Func<T, bool>> where = null) where T : class;
    IQueryable<T> Where<T>(Expression<Func<T, bool>> where, string entityName = "") where T : class;
    IQueryable<BsonDocument> Where(Expression<Func<BsonDocument, bool>> where, string entityName);
    IQueryable<T> Where<T>(Expression<Func<BsonDocument, bool>> where, string entityName, Expression<Func<BsonDocument, T>> convert) where T : class;

    void Remove<T>(T input, string id) where T : class;
    void Remove<T>(T input, string entityName, string id) where T : class;
    void Remove<T>(IEnumerable<UpdateEntity<T>> items) where T : class;
    void Remove<T>(IEnumerable<UpdateEntity<T>> items, string entityName) where T : class;
  }
}
