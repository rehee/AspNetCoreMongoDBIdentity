using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Content
{
  public abstract class ContentBase : SDHCCBaseEntity
  {
    public string ParentId { get; set; } = "";
    public List<string> Children { get; set; } = new List<string>();
    private string name { get; set; }
    public string Name
    {
      get
      {
        return name;
      }
      set
      {
        name = value.Replace('/', '_');
      }
    }

    public DateTime CreateTime { get; set; }
    public int SortOrder { get; set; } = 0;
  }

  public class ContentBaseModel : ContentBase
  {

  }
  public static class ContentE
  {
    public static Type RootType { get; set; }
  }
}
