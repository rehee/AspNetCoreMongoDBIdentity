using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
  public class SiteSetting
  {
    public string Name { get; set; }
    public string Password { get; set; }
    public string AdminRole { get; set; } = "Admin";
    public string BackUser { get; set; } = "BackUser";
    public string Login { get; set; } = "/Account/Login/";
  }
  
}
