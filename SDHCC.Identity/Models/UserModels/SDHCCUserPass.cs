using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.Identity.Models.UserModels
{
  public class SDHCCUserPass: SDHCCIdentityBase, IPassModel
  {
    public List<ContentProperty> Properties { get; set; } = new List<ContentProperty>();
  }
}
