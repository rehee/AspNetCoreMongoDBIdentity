using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Models.UserModels
{
  public class UserRoleView
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Roles { get; set; }
  }
}
