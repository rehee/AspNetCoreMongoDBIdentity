using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Models.UserModels
{
  public class DefaultUserSetting
  {
    public string Name { get; set; }
    public string Password { get; set; }
    public string AdminRole { get; set; } = "Admin";
    public string BackUser { get; set; } = "BackUser";
    public string Login { get; set; } = "/Account/Login/";
  }
}
