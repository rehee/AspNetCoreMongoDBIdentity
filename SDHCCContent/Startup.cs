using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SDHCC.DB;
using SDHCC.DB.Content;
using SDHCC.DB.Models;
using SDHCC.Identity;
using SDHCC.Identity.Models.UserRoles;

namespace SDHCCContent
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      //TelemetryConfiguration.Active.DisableTelemetry = true;
      //services.Configure<CookiePolicyOptions>(options =>
      //{
      //  // This lambda determines whether user consent for non-essential cookies is needed for a given request.
      //  options.CheckConsentNeeded = context => true;
      //  options.MinimumSameSitePolicy = SameSiteMode.None;
      //});

      services.AddSingleton<IMongoDatabase>(s =>
      {
        var client = new MongoClient(@"mongodb+srv://rehee_1:rehee_1_psw@cluster0-igkz0.gcp.mongodb.net/test?retryWrites=true");
        var database = client.GetDatabase("lalala");
        return database;
      });
      SDHCCBaseEntity.db = () =>
      {
        var client = new MongoClient(@"mongodb+srv://rehee_1:rehee_1_psw@cluster0-igkz0.gcp.mongodb.net/test?retryWrites=true");
        var database = client.GetDatabase("lalala");
        return database;
      };
      SDHCCBaseEntity.context = new SDHCCDbContext(SDHCCBaseEntity.db());


      ContentE.RootType = typeof(SDHCCContent.Models.ContentBaseModel);
      services.AddScoped<ISDHCCDbContext, SDHCCDbContext>();
      services.AddScoped<IRoleStore<IdentityRole>, SDHCCRoleStore<IdentityRole, SDHCUserRole>>();
      //services.AddScoped<IUserRoleStore<MUser>, SDHCCUserRoleStore<MUser, MRole, MUserRole>>();
      services.AddScoped<IUserStore<IdentityUser>, SDHCCUserStore<IdentityUser, IdentityRole, SDHCUserRole>>();

      services.AddScoped<UserManager<IdentityUser>>();
      services.AddScoped<RoleManager<IdentityRole>>();
      //services.AddIdentity<IdentityUser, IdentityRole>(option =>
      //{

      //}).AddDefaultTokenProviders();



      services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders();
      //.AddEntityFrameworkStores<ApplicationDbContext>();

      services.AddMvc()
        .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseDeveloperExceptionPage();
      app.UseDatabaseErrorPage();
      //if (env.IsDevelopment())
      //{
      //  app.UseDeveloperExceptionPage();
      //  app.UseDatabaseErrorPage();
      //}
      //else
      //{
      //  app.UseExceptionHandler("/Home/Error");
      //  app.UseHsts();
      //}

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      //app.UseCookiePolicy();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "areas",
          template: "{area}/{controller=Home}/{action=Index}/{id?}"
        );
        //routes.MapRoute("content", "{*names}",
        //    defaults: new { controller = "Content", action = "Index" });
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
  public class PascalCaseJsonProfileFormatter : JsonOutputFormatter
  {
    public PascalCaseJsonProfileFormatter() : base(new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() }, ArrayPool<char>.Shared)
    {
      SupportedMediaTypes.Clear();
      SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json;profile=\"https://en.wikipedia.org/wiki/PascalCase\""));
    }
  }
}
