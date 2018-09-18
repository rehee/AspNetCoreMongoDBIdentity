using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB
{
  public partial class SDHCCDbContext : ISDHCCDbContext
  {
    public void AddContent(ContentPassingModel content)
    {
      var obj = content.ConvertToBaseModel();
      var parent = (ContentBase)Find(obj.ParentId, "ContentBase", obj.FullType, out var response);
      if (parent == null)
      {
        obj.ParentId = "";
      }
      try
      {
        var collection = db.GetCollection<ContentBase>("ContentBase");
        collection.InsertOne(obj);
      }
      catch (Exception ex)
      {

      }
      if (parent != null)
      {
        parent.Children.Add(obj.Id);
        Update(parent, parent.Id, "ContentBase", out response);
      }
    }
  }
}
