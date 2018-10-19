using MongoDB.Bson.Serialization.Attributes;
using SDHCC;
using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCAuth.Models
{
  [AllowChildren(ChildrenType = new Type[] { typeof(Home), typeof(Page) })]
  public abstract class MVCAuthBase : ContentBase
  {

  }

  [AllowChildren(ChildrenType = new Type[] { typeof(Page) })]
  public class Home : MVCAuthBase
  {
    [Display(Name = "This is the Name")]
    [Required]
    public string Title { get; set; }

    [InputType(EditorType = EnumInputType.TextArea)]
    public string BriefContent { get; set; }


    public int PageView { get; set; } = 10;
  }
  public class Page : MVCAuthBase
  {

    [Display(Name = "This is the page Title")]
    public string Title { get; set; }
  }
}
