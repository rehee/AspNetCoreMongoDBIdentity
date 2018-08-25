using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SDHCC.Core.MethodResponse;
using SDHCC.DB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    public void Add<T>(T input, out MethodResponse response) where T : BaseEntity
    {
      response = new MethodResponse();
      if (String.IsNullOrEmpty(input.Id))
      {
        input.Id = Guid.NewGuid().ToString();
      }
      try
      {
        var typeName = input.GetType().Name;
        var collection = db.GetCollection<T>(typeName);
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

    public void AddRange<T>(IEnumerable<T> input, out MethodResponse response) where T : BaseEntity
    {
      foreach(var item in input)
      {
        this.Add<T>(item, out var res);
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
    public T Find<T>(string key, out MethodResponse response) where T: BaseEntity
    {
      response = new MethodResponse();
      try
      {
        var typeName = typeof(T).Name;
        var collection = db.GetCollection<T>(typeName);
        T obj = default(T);
        try
        {
          var objId = ObjectId.Parse(key);
          obj = collection.Find(new { Id = objId }.ToBsonDocument()).FirstOrDefault();
        }
        catch
        {
          obj = collection.Find(new { _id = key }.ToBsonDocument()).FirstOrDefault();
        }
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
    public IEnumerable<T> Find<T>(IEnumerable<string> keys, out MethodResponse response) where T: BaseEntity
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
    public T Find<T>(object search, out MethodResponse response) where T : BaseEntity
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
    public IEnumerable<T> Filter<T>(FilterParam param, out MethodResponse response) where T : BaseEntity
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
    public void Update<T>(T input,out MethodResponse response) where T : BaseEntity
    {
      response = new MethodResponse();
      try
      {
        var name = typeof(T).Name;
        var collection = db.GetCollection<T>(name);
        collection.UpdateOne(new { Id = input.Id }.ToBsonDocument(), new BsonDocument { { "$set", input.ToBsonDocument() } });
        response.Success = true;
      }catch(Exception ex)
      {
        response.ResponseMessage = ex.Message;
        response.ResponseObject = ex;
      }
    }
    public IQueryable<T> Where<T>(Expression<Func<T, bool>> where = null) where T : BaseEntity
    {
      var name = typeof(T).Name;
      var collection = db.GetCollection<T>(name);
      if (where != null)
      {
        var query = collection.AsQueryable<T>();
        return query.Where(where);
      }
      return collection.AsQueryable<T>();
    }

    public void Remove<T>(T input) where T : BaseEntity
    {
      try
      {
        var name = input.GetType().Name;
        var collection = db.GetCollection<T>(name);
        collection.DeleteOne(new { Id = input.Id }.ToBsonDocument());
      }
      catch { }
    }
    public void Remove<T>(IEnumerable<T> input) where T : BaseEntity
    {
      foreach(var item in input)
      {
        this.Remove<T>(item);
      }
    }
  }
}
