﻿using SDHCC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SDHCCContent.Models
{
  [AllowChildren(ChildrenType = new Type[] { typeof(Page) })]
  public class Home: ContentBaseModel
  {
    [Display(Name = "This is the Name")]
    public string Title { get; set; }

    [InputType(EnumInputType.TextArea)]
    public string BriefContent { get; set; }
  }
}
