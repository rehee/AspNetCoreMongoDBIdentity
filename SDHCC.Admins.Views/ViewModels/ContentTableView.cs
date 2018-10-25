using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC.Admins.Views.ViewModels
{
  public class ContentTableView
  {
    public IEnumerable<BsonDocument> Contents { get; set; } = Enumerable.Empty<BsonDocument>();
    public string RowClickFunction { get; set; } = "";
    public IEnumerable<ItemKeyAndName> Keys { get; set; } = Enumerable.Empty<ItemKeyAndName>();
    public ContentTableView(IEnumerable<BsonDocument> contents, string rowClickFunction = "", Type contentType = null)
    {
      if (contents != null)
        Contents = contents;
      if (!string.IsNullOrEmpty(rowClickFunction))
        RowClickFunction = rowClickFunction;
      if (contentType != null)
      {
        Keys = new List<ItemKeyAndName>();
        var listItem = contentType.GetObjectCustomAttribute<ListItemAttribute>();
        if (listItem != null && listItem.KeyAndDisplayNames != null)
        {
          foreach(var k in listItem.KeyAndDisplayNames)
          {
            var kAndNames = k.Split(",");
            ((IList<ItemKeyAndName>)Keys).Add(new ItemKeyAndName(
              kAndNames[0],kAndNames.Count()>1?kAndNames[1]: kAndNames[0]));
          }
        }
      }
    }
  }

  public class ItemKeyAndName
  {
    public string Key { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public ItemKeyAndName(string key, string displayName = null)
    {
      Key = key;
      if (string.IsNullOrEmpty(displayName))
      {
        DisplayName = key;
      }
      else
      {
        DisplayName = displayName;
      }
    }
  }
}
