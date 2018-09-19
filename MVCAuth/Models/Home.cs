using SDHCC;
using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCAuth.Models
{
  public class Home : ContentBase
  {
    public override string AssemblyName
    {
      get
      {
        return "MVCAuth";
      }
      set
      {

      }
    }
    [Display(Name = "This is the Name")]
    [InputType(EnumInputType.Text)]
    public string Name { get; set; }
    [Required]
    public string Title { get; set; }

    [InputType(EnumInputType.TextArea)]
    public string BriefContent { get; set; }


    public int PageView { get; set; } = 10;
  }
}
