using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC
{
  public class InputTypeAttribute : Attribute
  {
    public EnumInputType EditorType { get; set; } = EnumInputType.Text;
    public bool MultiSelect { get; set; } = false;
    public Type RelatedType { get; set; } = null;
    public int RangeMin { get; set; } = 0;
    public int RangeMax { get; set; } = 100;
    public bool RangeMaxSelf { get; set; } = false;

    public InputTypeAttribute()
    {

    }
  }
}
