using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDHCC.DB.Content
{
  public static partial class DropDownExtendConvert
  {
    public static DropDownPostModel ConvertToPassingModel(this DropDownBase input)
    {
      var result = new DropDownPostModel();
      var resultProperty = result.GetType().GetProperties().Where(b => b.BaseProperty()).ToList();
      var properties = input.GetType().GetProperties();
      foreach (var p in properties)
      {
        if (p.BaseProperty())
        {
          var baseP = resultProperty.Where(b => b.Name == p.Name).FirstOrDefault();
          baseP.SetValue(result, p.GetValue(input));
          continue;
        }
        if (p.SkippedProperty())
          continue;
        result.Properties.Add(p.GetContentPropertyByPropertyInfo(input));
      }
      return result;
    }
    public static DropDownBase ConvertToBaseModel(this DropDownPostModel input)
    {
      var result = (DropDownBase)input.ConvertBaseTypeToT(out var typeName, out var assemblyName);
      var properties = result.GetType().GetProperties();
      var baseProperty = input.GetType().GetProperties().Where(b => b.BaseProperty()).ToList();
      foreach (var p in properties)
      {
        try
        {
          if (p.BaseProperty())
          {
            var inputBaseProperty = baseProperty.Where(b => b.Name == p.Name).FirstOrDefault();
            if (inputBaseProperty == null)
              continue;
            p.SetValue(result, inputBaseProperty.GetValue(input));
            continue;
          }

          if (p.SkippedProperty())
            continue;
          p.SetPropertyValue(input, result);
        }
        catch { }

      }
      result.FullType = typeName;
      result.AssemblyName = assemblyName;
      return result;
    }
    
  }
}
