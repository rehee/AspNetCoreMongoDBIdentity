using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC
{
  public class AllowChildrenAttribute : Attribute
  {
    public Type[] ChildrenType { get; set; }
    public AllowChildrenAttribute()
    {
    }
  }
}
