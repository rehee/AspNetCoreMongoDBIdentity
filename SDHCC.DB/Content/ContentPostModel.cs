using SDHCC.DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC.DB.Content
{
  public class ContentPostModel : BaseProperty, IPassModel
  {
    public List<ContentProperty> Properties { get; set; } = new List<ContentProperty>();
  }

  public interface IPassModel
  {
    List<ContentProperty> Properties { get; set; }
  }
  public class ContentProperty
  {
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
    public string ValueType { get; set; } = "";
    public string Title { get; set; } = "";
    public EnumInputType EditorType { get; set; } = EnumInputType.Text;
    public bool MultiSelect { get; set; } = false;
    public Type RelatedType { get; set; } = null;
  }

  public class ContentPropertyIndex
  {
    public ContentProperty Property { get; set; }
    public int Index { get; set; }
    public ContentPropertyIndex(ContentProperty property, int index)
    {
      Property = property;
      Index = index;
    }
  }
}
