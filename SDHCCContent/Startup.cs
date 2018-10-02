using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using SDHCC.Identity.Models.UserModels;
using SDHCC.Identity.Models.UserRoles;
using SDHCC.Identity.Services;

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
      E.Setting = Configuration.GetSection("SiteSetting").Get<SiteSetting>();
      //TelemetryConfiguration.Active.DisableTelemetry = true;
      //services.Configure<CookiePolicyOptions>(options =>
      //{
      //  // This lambda determines whether user consent for non-essential cookies is needed for a given request.
      //  options.CheckConsentNeeded = context => true;
      //  options.MinimumSameSitePolicy = SameSiteMode.None;
      //});
      var dbConnect = @"mongodb+srv://rehee_1:rehee_1_psw@cluster0-igkz0.gcp.mongodb.net/test?retryWrites=true";
      dbConnect = Configuration.GetConnectionString("DefaultConnection");
      var dbName = Configuration["DatabaseName"];

      services.AddSingleton<IMongoDatabase>(s =>
      {
        var client = new MongoClient(dbConnect);
        var database = client.GetDatabase(dbName);
        return database;
      });
      SDHCCBaseEntity.db = () =>
      {
        var client = new MongoClient(dbConnect);
        var database = client.GetDatabase(dbName);
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
      services.AddScoped<ISDHCCIdentity, SDHCCIdentity<IdentityUser>>();
      services.AddIdentity<IdentityUser, IdentityRole>(options =>
      {
        
      }).AddDefaultTokenProviders();
      services.Configure<CookieAuthenticationOptions>(options =>
      {
        options.LoginPath = new PathString("/login");
      });


      //services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders();
      //.AddEntityFrameworkStores<ApplicationDbContext>();

      services.AddMvc()
        .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
          options.LoginPath = E.Setting.Login;
        });
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
        routes.MapRoute("content", "{*names}",
            defaults: new { controller = "Content", action = "Index" });
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
