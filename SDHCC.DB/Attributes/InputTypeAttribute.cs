using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC
{
  public class InputTypeAttribute : Attribute
  {
    public EnumInputType InputType { get; set; }
    public InputTypeAttribute(EnumInputType type)
    {
      InputType = type;
    }
  }
}
