using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using Newtonsoft.Json.Serialization;
using SDHCC;
using SDHCC.DB;
using SDHCC.DB.Content;
using SDHCC.DB.Models;
using SDHCC.Identity;
using SDHCC.Identity.Models.UserModels;
using SDHCC.Identity.Models.UserRoles;
using SDHCC.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
  public static class StartUpFunction
  {
    public static void ConfigureServices<TUser, TContentBase, TDropDownBase>(IServiceCollection services,
      IConfiguration configuration, IHostingEnvironment hostingEnvironment, bool loadAdminView = true)
      where TUser : SDHCCUserBase, new()
      where TContentBase : ContentBase
      where TDropDownBase : DropDownBase
    {
      var a = configuration.GetConnectionString("DefaultConnection");
      E.Setting = configuration.GetSection("SiteSetting").Get<SiteSetting>();

      if (hostingEnvironment.IsDevelopment())
      {
        E.Setting = configuration.GetSection("SiteSetting").Get<SiteSetting>();
      }
      else
      {
        E.Setting = configuration.GetSection("SiteSetting").Get<SiteSetting>();
      }
      //TelemetryConfiguration.Active.DisableTelemetry = true;
      //services.Configure<CookiePolicyOptions>(options =>
      //{
      //  // This lambda determines whether user consent for non-essential cookies is needed for a given request.
      //  options.CheckConsentNeeded = context => true;
      //  options.MinimumSameSitePolicy = SameSiteMode.None;
      //});
      var dbConnect = @"mongodb+srv://rehee_1:rehee_1_psw@cluster0-igkz0.gcp.mongodb.net/test?retryWrites=true";
      dbConnect = configuration.GetConnectionString("DefaultConnection");
      var dbName = configuration["DatabaseName"];

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
      E.RootPath = hostingEnvironment.WebRootPath;
      ContentE.RootPath = hostingEnvironment.WebRootPath;

      ContentE.RootType = typeof(TContentBase);
      ContentE.RootDropDown = typeof(TDropDownBase);
      ContentE.FileUploadPath = E.Setting.FileUploadPath;
      services.AddScoped<ISDHCCDbContext, SDHCCDbContext>();
      services.AddScoped<IRoleStore<IdentityRole>, SDHCCRoleStore<IdentityRole, SDHCUserRole>>();
      //services.AddScoped<IUserRoleStore<MUser>, SDHCCUserRoleStore<MUser, MRole, MUserRole>>();

      services.AddScoped<IUserStore<TUser>, SDHCCUserStore<TUser, IdentityRole, SDHCUserRole>>();
      services.AddScoped<UserManager<TUser>>();

      services.AddScoped<RoleManager<IdentityRole>>();
      services.AddScoped<ISDHCCIdentity, SDHCCIdentity<TUser>>();
      services.AddIdentity<TUser, IdentityRole>(options =>
      {

      }).AddDefaultTokenProviders();
      services.Configure<CookieAuthenticationOptions>(options =>
      {
        options.LoginPath = new PathString("/login");
      });


      //services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders();
      //.AddEntityFrameworkStores<ApplicationDbContext>();
      var assembly = typeof(SDHCC.Admins.Controllers.PageController).Assembly;
      var assemblyView = Assembly.Load("SDHCC.Admins.Views");
      services.AddMvc()
        .AddApplicationPart(assembly)
        //.ConfigureRazorViewEngine(options =>
        //{
        //  var oldRoot = ApplicationEnviroment.ApplicationBasePath;
        //  var trimmedRoot = oldRoot.Remove(oldRoot.LastIndexOf('\\'));

        //  options.FileProvider = new PhysicalFileProvider(trimmedRoot);
        //})
        ////.AddApplicationPart(assemblyView)
        //.ConfigureApplicationPartManager(m =>
        //{
        //  var feature = new ControllerFeature();
        //  m.ApplicationParts.Add(new AssemblyPart(assembly));
        //  m.PopulateFeature(feature);
        //  services.AddSingleton(feature.Controllers.Select(t => t.AsType()).ToArray());
        //})
        //.AddRazorOptions(options =>
        //{
        //  options.AddCloudscribeSimpleContentBootstrap3Views(assembly);
        //})
        .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
          options.LoginPath = E.Setting.Login;
        });
      if (loadAdminView)
      {
        services.Configure<RazorViewEngineOptions>(options =>
        {
          options.FileProviders.Add(
              new EmbeddedFileProvider(assemblyView, "SDHCC.Admins.Views"));
        });

      }

    }

    public static void Configure(IApplicationBuilder app, IHostingEnvironment env, bool loadAdminView = true)
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

      if (loadAdminView)
      {
        var assemblyView = Assembly.Load("SDHCC.Admins.Views");
        var personEmbeddedFileProvider = new EmbeddedFileProvider(
          assemblyView
        );
        app.UseStaticFiles(new StaticFileOptions
        {
          FileProvider = personEmbeddedFileProvider,
          RequestPath = ""
        });
      }
      //app.UseCookiePolicy();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "areas",
          template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
        );
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
        routes.MapRoute("content", "{*names}",
            defaults: new { controller = "Content", action = "Index" });
      });
    }
  }
}
