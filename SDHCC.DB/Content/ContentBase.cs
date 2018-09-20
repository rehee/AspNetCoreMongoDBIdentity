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

    public virtual string Name { get; set; }
  }

  public class ContentBaseModel: ContentBase
  {

  }
  public static class ContentE
  {
    public static Type RootType { get; set; }
  }
}
