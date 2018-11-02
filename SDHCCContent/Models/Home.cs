using MongoDB.Bson.Serialization.Attributes;
using SDHCC;
using SDHCCContent.Models.DropDowns;
using SDHCCContent.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SDHCCContent.Models
{
  [AllowChildren(ChildrenType = new Type[] { typeof(Page) })]
  [ListItem(KeyAndDisplayNames = new string[] { "Title,ttt" })]
  public class Home : ContentBaseModel
  {
    [Display(Name = "名字")]
    public string Title { get; set; }

    [InputType(EditorType = EnumInputType.TextArea)]
    public string BriefContent { get; set; }

    [InputType(EditorType = EnumInputType.DateTime)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
    public DateTime LastEdit { get; set; }

    [InputType(EditorType = EnumInputType.DropDwon, RelatedType = typeof(EnumGender))]
    public EnumGender Gender { get; set; }

    [InputType(EditorType = EnumInputType.DropDwon, RelatedType = typeof(DropDownGender))]
    public string Genders { get; set; }

    [InputType(EditorType = EnumInputType.DropDwon, RelatedType = typeof(EnumGender), MultiSelect = true)]
    public List<EnumGender> Genderss { get; set; }

    [InputType(EditorType = EnumInputType.DropDwon, RelatedType = typeof(DropDownGender), MultiSelect = true)]
    public List<string> Gendersss { get; set; }
    [InputType(EditorType = EnumInputType.FileUpload)]
    public string UploadFile { get; set; }

  }
}
