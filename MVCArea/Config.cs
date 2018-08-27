using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MVCArea
{
  public static class Config
  {
    public static void Mvc(IServiceCollection services)
    {
      services.AddMvc().ConfigureApplicationPartManager(m =>
      {
        string nspace = "MVCArea.Areas.Controllers";

        var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.IsClass && t.Namespace == nspace
                select t;
        var feature = new ControllerFeature();
        foreach (var t in q)
        {
          //var homeType = typeof(MVCArea.Areas.Controllers.HomeController);
          var controllerAssembly = t.Assembly;
          m.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
        }
        m.PopulateFeature(feature);
        services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
      }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }
  }
}
