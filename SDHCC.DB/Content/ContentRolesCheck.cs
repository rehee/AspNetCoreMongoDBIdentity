using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC.DB.Content
{
  public class ContentRolesCheck
  {
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string PropertyName { get; set; }
    public ContentRolesCheck(IEnumerable<string> roles,string propertyName)
    {
      Roles = roles.Select(b => b).ToList();
      PropertyName = propertyName;
    }
  }
}
