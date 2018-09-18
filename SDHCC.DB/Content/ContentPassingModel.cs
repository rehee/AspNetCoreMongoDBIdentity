using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Content
{
  public class ContentPassingModel
  {
    public string Id { get; set; }
    public string ParentId { get; set; }
    public string FullType { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
  }

  public class ContentValue
  {
    public string Key { get; set; }
    public string Value { get; set; }
    public string TypeName { get; set; }
  }
}
