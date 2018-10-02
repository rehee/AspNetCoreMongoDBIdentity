using SDHCC;
using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDHCCContent.Models
{
  [AllowChildren(
    ChildrenType = new Type[] { typeof(Home), typeof(Page) },
    CreateRoles = new string[] { "Admin" },
    EditRoles = new string[] { "Admin" })]
  public abstract class ContentBaseModel : ContentBase
  {

  }
}
