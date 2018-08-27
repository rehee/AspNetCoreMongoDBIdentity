using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCArea
{
  public static class Config
  {
    public static void Mvc(IServiceCollection services)
    {
      services.AddMvc().ConfigureApplicationPartManager(m =>
      {
        var homeType = typeof(MVCArea.Areas.Controllers.HomeController);
        var controllerAssembly = homeType.Assembly;
        var feature = new ControllerFeature();
        m.ApplicationParts.Add(new AssemblyPart(controllerAssembly));
        m.PopulateFeature(feature);
        services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
      }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    }
  }
}
