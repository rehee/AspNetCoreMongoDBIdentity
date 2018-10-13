using MongoDB.Bson;
using SDHCC.DB.Content;
using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC.DB
{
  public partial interface ISDHCCDbContext
  {
    IEnumerable<DropDownSummary> GetDropDownSummary();
    IQueryable<DropDownBase> GetDropDownsByName(string name);
    void AddDropDown(DropDownBase model);
    void UpdateDropDown(DropDownBase model, IEnumerable<string> ignoreKeys = null, IEnumerable<string> takeKeys = null);
    void RemoveDropDown(DropDownBase model);
  }
}
