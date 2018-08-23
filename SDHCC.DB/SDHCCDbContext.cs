using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SDHCC.Core.MethodResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC.DB
{
  public class SDHCCDbContext : ISDHCCDbContext
  {
    private IMongoDatabase db { get; set; }
    public SDHCCDbContext(IMongoDatabase db)
    {
      this.db = db;
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
    public T Find<T>(string key, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var typeName = typeof(T).Name;
        var collection = db.GetCollection<BsonDocument>(typeName);
        var obj = collection.Find(new { Id = ObjectId.Parse(key) }.ToBsonDocument()).FirstOrDefault();
        if (obj == null)
        {
          response.Success = true;
          return default(T);
        }
        var entity = BsonSerializer.Deserialize<T>(obj);
        response.Success = true;
        return entity;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
        return default(T);
      }
    }
    public IEnumerable<T> Find<T>(IEnumerable<string> keys, out MethodResponse response)
    {
      var result = new List<T>();
      var uniqueKeys = keys.Distinct();
      foreach (var key in uniqueKeys)
      {
        MethodResponse res;
        var obj = this.Find<T>(key, out res);
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
      response = new MethodResponse();
      response.Success = true;
      return result;
    }
    public object Find(string key, string entityName, string fullName, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var collection = db.GetCollection<BsonDocument>(entityName);
        var obj = collection.Find(new { Id = ObjectId.Parse(key) }.ToBsonDocument()).FirstOrDefault();
        if (obj == null)
        {
          response.Success = true;
          return null;
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
    public T Find<T>(object search, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var typeName = typeof(T).Name;
        var collection = db.GetCollection<BsonDocument>(typeName);
        var obj = collection.Find(search.ToBsonDocument()).FirstOrDefault();
        if (obj == null)
        {
          response.Success = true;
          return default(T);
        }
        var entity = BsonSerializer.Deserialize<T>(obj);
        response.Success = true;
        return entity;
      }
      catch (Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
        return default(T);
      }
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
    public IEnumerable<T> Filter<T>(FilterParam param, out MethodResponse response)
    {
      response = new MethodResponse();
      try
      {
        var typeName = typeof(T).Name;
        var collection = db.GetCollection<BsonDocument>(typeName);
        var obj = collection.Find(param.Filters.GetFilter()).ToList();
        if (obj == null)
        {
          response.Success = true;
          return new List<T>();
        }
        var entity = obj.Select(b => BsonSerializer.Deserialize<T>(b)).ToList();
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

  }
}
