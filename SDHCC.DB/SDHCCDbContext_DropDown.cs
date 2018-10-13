using MongoDB.Bson;
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
    public IEnumerable<DropDownSummary> GetDropDownSummary()
    {
      var root = ContentE.RootDropDown.CustomAttributes
        .Where(b => b.AttributeType == typeof(AllowChildrenAttribute))
        .FirstOrDefault();
      if (root == null)
        return Enumerable.Empty<DropDownSummary>();
      var dropDownTypes = ((System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument>)root.NamedArguments
        .Where(b => b.MemberName == "ChildrenType")
        .FirstOrDefault()
        .TypedValue.Value)
        .Select(b => b.Value as Type)
        .ToList();
      var result = new List<DropDownSummary>();
      foreach (var item in dropDownTypes)
      {
        var dropType = new DropDownSummary();
        dropType.Count = this.Where(b => true, item.Name).Count();
        dropType.DropDownName = item.Name;
        result.Add(dropType);
      }
      return result;
    }
    public IQueryable<DropDownBase> GetDropDownsByName(string name)
    {
      var query = Where(b => true, name, this.ConvertBsonToGeneric<DropDownBase>());
      return query;
    }
    public void AddDropDown(DropDownBase model)
    {
      if (String.IsNullOrEmpty(model.Id))
        model.GenerateId();
      Add<DropDownBase>(model, model.GetType().Name, out var response);
    }
    public void UpdateDropDown(DropDownBase model, IEnumerable<string> ignoreKeys = null, IEnumerable<string> takeKeys = null)
    {
      Update<DropDownBase>(model, model.Id, model.GetType().Name, ignoreKeys, takeKeys, out var response);
    }
    public void RemoveDropDown(DropDownBase model)
    {
      Remove<DropDownBase>(model, model.GetType().Name, model.Id);
    }
  }
}
