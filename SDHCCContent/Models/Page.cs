using SDHCC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SDHCCContent.Models
{
  public class Page : ContentBaseModel
  {
    [Display(Name = "This is the Name")]
    public string Title { get; set; }

    [InputType(EditorType = EnumInputType.TextArea)]
    public string BriefContent { get; set; }
  }
}
