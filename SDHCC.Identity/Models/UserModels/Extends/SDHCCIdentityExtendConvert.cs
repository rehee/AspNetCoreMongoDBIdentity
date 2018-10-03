using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDHCC.DB.Content;
namespace SDHCC.Identity.Models.UserModels
{
  public static class SDHCCIdentityExtendConvert
  {
    public static SDHCCUserPass ConvertUserToPass(this SDHCCUserBase input)
    {
      var result = new SDHCCUserPass();
      var inputProperties = input.GetType().GetProperties();
      var resultProperties = result.GetType().GetProperties();

      foreach (var p in inputProperties)
      {
        if (!p.CustomProperty())
        {
          var targetPropertyInfo = resultProperties.Where(b => b.Name == p.Name).FirstOrDefault();
          if (targetPropertyInfo == null)
            continue;
          targetPropertyInfo.SetValue(result, p.GetValue(input));
        }
        else
        {
          result.Properties.Add(p.GetContentPropertyByPropertyInfo(input));
        }
      }
      return result;
    }

    public static SDHCCUserBase ConvertPassToUser(this SDHCCUserPass input)
    {
      var result = (SDHCCUserBase)input.ConvertBaseTypeToT(out var typeName, out var assemblyName);
      var inputProperties = input.GetType().GetProperties();
      var resultProperties = result.GetType().GetProperties();
      foreach (var p in resultProperties)
      {
        if (!p.CustomProperty())
        {
          var targetPropertyInfo = inputProperties.Where(b => b.Name == p.Name).FirstOrDefault();
          if (targetPropertyInfo == null)
            continue;
          p.SetValue(result, targetPropertyInfo.GetValue(input));
          continue;
        }
        p.SetPropertyValue(input, result);
      }
      result.AssemblyName = assemblyName;
      result.FullType = typeName;
      return result;
    }


  }
}
