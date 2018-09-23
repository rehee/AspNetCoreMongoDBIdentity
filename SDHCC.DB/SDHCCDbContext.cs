using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SDHCC.Core.MethodResponse;
using SDHCC.DB.Content;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SDHCC.DB
{
  public partial class SDHCCDbContext : ISDHCCDbContext
  {
    private IMongoDatabase db { get; set; }
    public SDHCCDbContext(IMongoDatabase db)
    {
      this.db = db;
    }

    public Expression<Func<BsonDocument, T>> ConvertBsonToGeneric<T>()
    {
      return bson => BsonConvertTo<T>(bson);
    }
    private T BsonConvertTo<T>(BsonDocument bson)
    {
      if (bson == null)
      {
        return default(T);
      }
      return (T)BsonSerializer.Deserialize(bson, GetType<T>(bson));
    }
    private Type GetType<T>(BsonDocument bson)
    {
      var fullType = "";
      Type finalType = typeof(T);
      try
      {
        fullType = $"{bson["FullType"].ToString()},{bson["AssemblyName"].ToString()}";
        finalType = Type.GetType(fullType);
      }
      catch { }
      if (finalType == null)
      {
        return typeof(ContentBaseModel);
      }
      return finalType;
    }

    public void Add<T>(T input, out MethodResponse response) where T : class
    {
      var typeName = input.GetType().Name;
      Add<T>(input, typeName, out response);

    }
    public void Add<T>(T input, string entityName, out MethodResponse response) where T : class
    {
      response = new MethodResponse();
      try
      {
        var collection = db.GetCollection<T>(entityName);
        collection.InsertOne(input);
        response.Success = true;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
      }
    }
    public void Add(object input, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var bsonObject = input.ToBsonDocument();
        var typeName = input.GetType().Name;
        var collection = db.GetCollection<BsonDocument>(typeName);
        collection.InsertOne(bsonObject);
        response.Success = true;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
      }

    }

    public void AddRange<T>(IEnumerable<T> input, out MethodResponse response) where T : class
    {
      foreach (var item in input)
      {
        this.Add<T>(item, out var res);
      }
      response = new MethodResponse();
      response.Success = true;
    }
    public void AddRange<T>(IEnumerable<T> input, string entityName, out MethodResponse response) where T : class
    {
      foreach (var item in input)
      {
        this.Add<T>(item, entityName, out var res);
      }
      response = new MethodResponse();
      response.Success = true;
    }
    public void AddRange(IEnumerable<object> input, out MethodResponse response)
    {
      foreach (var item in input)
      {
        this.Add(item, out var res);
        if (res.Success != true)
        {
          response = res;
          return;
        }
      }
      response = new MethodResponse();
      response.Success = true;
    }

    public T Find<T>(string key, out MethodResponse response) where T : class
    {
      var typeName = typeof(T).Name;
      return Find<T>(key, typeName, out response);
    }
    public T Find<T>(string key, string entityName, out MethodResponse response) where T : class
    {
      return Find<T>(key, entityName, ConvertBsonToGeneric<T>(), out response);
    }

    public IEnumerable<T> Find<T>(IEnumerable<string> keys, out MethodResponse response) where T : class
    {
      var entityName = typeof(T).Name;
      return Find<T>(keys, entityName, out response).AsEnumerable();
    }
    public IQueryable<T> Find<T>(IEnumerable<string> keys, string entityName, out MethodResponse response) where T : class
    {
      response = new MethodResponse();
      try
      {
        var strings = keys.Select(c => (object)c).ToList();
        var query = Where<T>(b => strings.Contains(b["_id"]), entityName, ConvertBsonToGeneric<T>());
        return query;
      }
      catch (Exception ex)
      {
        response.ResponseObject = ex;
        response.Message = ex.Message;
        return Enumerable.Empty<T>().AsQueryable();
      }

    }
    public object Find(string key, string entityName, string fullName, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var collection = db.GetCollection<BsonDocument>(entityName);
        BsonDocument obj = null;
        try
        {
          ObjectId oId = ObjectId.Parse(key);
          obj = collection.Find(new { Id = oId }.ToBsonDocument()).FirstOrDefault();
        }
        catch
        {
          obj = collection.Find(new { _id = key }.ToBsonDocument()).FirstOrDefault();
        }

        if (obj == null)
        {
          response.Success = true;
          return null;
        }
        if (String.IsNullOrEmpty(fullName))
        {
          fullName = $"{obj["FullType"].ToString()},{obj["AssemblyName"].ToString()}";
        }
        var returnObj = BsonSerializer.Deserialize(obj, Type.GetType(fullName));
        response.Success = true;
        return returnObj;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
        return null;
      }
    }

    public T Find<T>(object search, out MethodResponse response) where T : class
    {
      response = new MethodResponse();
      try
      {
        var typeName = typeof(T).Name;
        var collection = db.GetCollection<T>(typeName);
        var obj = collection.Find(search.ToBsonDocument()).FirstOrDefault();
        response.Success = true;
        return obj;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
        return default(T);
      }
    }
    public IEnumerable<object> Find(IEnumerable<string> keys, string entityName, string fullName, out MethodResponse response)
    {
      var result = new List<object>();
      var uniqueKeys = keys.Distinct();
      foreach (var key in uniqueKeys)
      {
        MethodResponse res;
        var obj = this.Find(key, entityName, fullName, out res);
        if (res.Success)
        {
          result.Add(obj);
        }
        else
        {
          response = res;
          return result;
        }
      }
      response = new MethodResponse() { Success = true };
      return result;
    }

    public object Find(SearchParam search, out MethodResponse response)
    {
      var filter = Builders<BsonDocument>.Filter.In("_id", "");
      response = new MethodResponse();
      try
      {
        var typeName = search.EntityName;
        var collection = db.GetCollection<BsonDocument>(typeName);
        var obj = collection.Find(search.SearchObject.ToBsonDocument()).FirstOrDefault();
        if (obj == null)
        {
          response.Success = true;
          return null;
        }
        var entity = BsonSerializer.Deserialize(obj, Type.GetType(search.ReturnFullName));
        response.Success = true;
        return entity;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
        return null;
      }
    }

    public BsonDocument Find(string key, string entityName, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var bson = Where(b => b["_id"] == key, entityName).FirstOrDefault();
        response.Success = true;
        return bson;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
        return null;
      }
    }
    public T Find<T>(string key, string entityName, Expression<Func<BsonDocument, T>> convert, out MethodResponse response)
    {
      var bson = Find(key, entityName, out response);
      if (bson == null)
      {
        return default(T);
      }
      return convert.Compile()(bson);
    }

    public IEnumerable<T> Filter<T>(FilterParam param, out MethodResponse response) where T : class
    {
      response = new MethodResponse();
      try
      {
        var typeName = typeof(T).Name;
        var collection = db.GetCollection<T>(typeName);
        var obj = collection.Find(param.Filters.GetFilter<T>()).ToList();
        response.Success = true;
        return obj;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
        return null;
      }
    }



    public void Update<T>(T input, string id, out MethodResponse response) where T : class
    {
      var name = typeof(T).Name;
      Update<T>(input, id, name, out response);
    }
    public void Update<T>(T input, string id, string entityName, out MethodResponse response) where T : class
    {
      Update(input, id, entityName, null, null, out response);
    }
    public void Update<T>(T input, string id, string entityName, IEnumerable<string> ignoreKeys, IEnumerable<string> takeKeys, out MethodResponse response) where T : class
    {
      response = new MethodResponse();
      try
      {
        var collection = db.GetCollection<T>(entityName);
        var updateDocument = input.ToBsonDocument();
        var keys = updateDocument.Elements.Select(b => b.Name).ToList();
        List<string> deleteKeys = new List<string>();
        if (takeKeys != null)
        {
          deleteKeys = keys.Where(b => !takeKeys.Contains(b)).ToList();
        }
        else
        {
          if (ignoreKeys != null)
          {
            deleteKeys = keys.Where(b => ignoreKeys.Contains(b)).ToList();
          }
        }
        deleteKeys.Add("_t");
        deleteKeys.Add("_id");
        deleteKeys.Add("FullType");
        deleteKeys.Add("AssemblyName");
        foreach (var k in deleteKeys)
        {
          try
          {
            updateDocument.Remove(k);
          }
          catch { }

        }
        collection.UpdateOne(
          new { Id = id }.ToBsonDocument(),
          new BsonDocument { { "$set", updateDocument } }
          );
        response.Success = true;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
      }
    }

    public IQueryable<T> Where<T>(Expression<Func<T, bool>> where = null) where T : class
    {
      var name = typeof(T).Name;
      return Where<T>(where, name);
    }
    public IQueryable<T> Where<T>(Expression<Func<T, bool>> where = null, string entityName = "") where T : class
    {
      var t = typeof(T);
      var name = entityName;
      if (String.IsNullOrEmpty(entityName))
      {
        name = typeof(T).Name;
      }
      var collection = db.GetCollection<T>(name);
      if (where != null)
      {
        var query = collection.AsQueryable<T>();
        return query.Where(where);
      }
      return collection.AsQueryable<T>();
    }
    public IQueryable<BsonDocument> Where(Expression<Func<BsonDocument, bool>> where, string entityName)
    {
      var collection = db.GetCollection<BsonDocument>(entityName);
      var query = collection.AsQueryable<BsonDocument>();
      if (where != null)
      {
        return query.Where(where);
      }
      return query;
    }
    public IQueryable<T> Where<T>(Expression<Func<BsonDocument, bool>> where, string entityName, Expression<Func<BsonDocument, T>> convert) where T : class
    {
      return Where(where, entityName).Select(convert.Compile()).AsQueryable();
    }

    public void Remove<T>(T input, string id) where T : class
    {
      var name = input.GetType().Name;
      Remove<T>(input, name, id);
    }
    public void Remove<T>(T input, string entityName, string id) where T : class
    {
      try
      {
        var collection = db.GetCollection<T>(entityName);
        collection.DeleteOne(new { Id = id }.ToBsonDocument());
      }
      catch { }
    }
    public void Remove<T>(IEnumerable<UpdateEntity<T>> items) where T : class
    {
      foreach (var item in items)
      {
        this.Remove<T>(item.Object, item.Key);
      }
    }
    public void Remove<T>(IEnumerable<UpdateEntity<T>> items, string entityName) where T : class
    {
      foreach (var item in items)
      {
        this.Remove<T>(item.Object, entityName, item.Key);
      }
    }

  }
}
