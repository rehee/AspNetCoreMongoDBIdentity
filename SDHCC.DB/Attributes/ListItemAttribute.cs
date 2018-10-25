using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC
{
  public class ListItemAttribute : Attribute
  {
    public ListItemAttribute()
    {

    }
    public string[] KeyAndDisplayNames { get; set; }
  }
  
}
