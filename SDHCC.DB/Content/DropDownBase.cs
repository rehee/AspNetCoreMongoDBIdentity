using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Content
{
  public abstract class DropDownBaseProperty : SDHCCBaseEntity
  {
    private string name { get; set; }
    [BaseProperty]
    public string Name
    {
      get
      {
        return name;
      }
      set
      {
        name = String.IsNullOrEmpty(value) ? this.Id : value.Trim().Replace('/', '_').ToLower();
      }
    }
    [BaseProperty]
    public int SortOrder { get; set; } = 0;
  }
  public abstract class DropDownBase : DropDownBaseProperty
  {

  }
  public class DropDownPostModel : DropDownBaseProperty, IPassModel
  {
    public List<ContentProperty> Properties { get; set; } = new List<ContentProperty>();
  }
}
