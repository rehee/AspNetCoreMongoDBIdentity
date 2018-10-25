using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
  public class SiteMapItem
  {
    public string Url { get; set; }
    public string Icon { get; set; }
    public string Title { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public IEnumerable<SiteMapItem> Children { get; set; }
  }
}
