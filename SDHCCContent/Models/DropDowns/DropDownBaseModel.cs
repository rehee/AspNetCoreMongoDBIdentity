using SDHCC;
using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDHCCContent.Models.DropDowns
{
  [AllowChildren(
    ChildrenType = new Type[] { typeof(DropDownGender) },
    CreateRoles = new string[] { "Admin" },
    EditRoles = new string[] { "Admin" })]
  public class DropDownBaseModel : DropDownBase
  {

  }
}

