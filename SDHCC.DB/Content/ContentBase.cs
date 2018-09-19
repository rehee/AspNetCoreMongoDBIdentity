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
  }
  public class ContentBaseType: ContentBase
  {

  }

  public class Home: ContentBase
  {
    public static string ppp { get; set; }
    public string Title { get; set; }
  }
  public class Home2 : ContentBase
  {
    public static string ppp { get; set; }
    public string Title2 { get; set; }
  }
}
