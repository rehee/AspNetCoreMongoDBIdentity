using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Models
{
  public abstract class BaseProperty : SDHCCBaseEntity
  {
    [BaseProperty]
    public string ParentId { get; set; } = "";
    [BaseProperty]
    public List<string> Children { get; set; } = new List<string>();
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
    public DateTime CreateTime { get; set; }
    [BaseProperty]
    public int SortOrder { get; set; } = 0;

    [BaseProperty]
    public bool Publish { get; set; } = true;
    [BaseProperty]
    public bool RequireLogin { get; set; } = false;
    [BaseProperty]
    public List<string> PublicAccessRoles { get; set; } = new List<string>();
    [BaseProperty]
    public List<string> AdminCreateRoles { get; set; } = new List<string>();
    [BaseProperty]
    public List<string> AdminReadRoles { get; set; } = new List<string>();
    [BaseProperty]
    public List<string> AdminUpdateRoles { get; set; } = new List<string>();
    [BaseProperty]
    public List<string> AdminDeleteRoles { get; set; } = new List<string>();
  }
}

