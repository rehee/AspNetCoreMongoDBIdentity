using SDHCC.DB.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SDHCC
{
  public static class GeneralExtendConvert
  {
    public static IPassModel ConvertObjectToIPassModel(this object input, IPassModel model = null)
    {
      if (input == null)
        return null;
      if (model == null)
      {
        model = new BasePassModel();
      }
      var type = input.GetType();
      foreach(var p in type.GetProperties())
      {
        var value = p.GetContentPropertyByPropertyInfo(input);
        if (value != null)
          model.Properties.Add(value);
      }
      return model;
    }
    public static void SetIPassModel(this IPassModel model,object obj)
    {
      if (model == null || obj == null)
        return;
      var type = obj.GetType();
      foreach(var p in type.GetProperties())
      {
        p.SetPropertyValue(model, obj);
      }
    }
  }
}
