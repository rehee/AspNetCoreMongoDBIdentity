using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using System.Text;

namespace System
{
  public static class StartUpFunction
  {
    public static void ConfigureServices<TUser, TContentBase, TDropDownBase>(IServiceCollection services,
      IConfiguration configuration, IHostingEnvironment hostingEnvironment)
      where TUser : SDHCCUserBase, new() 
      where TContentBase : ContentBase
      where TDropDownBase: DropDownBase
    {
      E.Setting = configuration.GetSection("SiteSetting").Get<SiteSetting>();
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

      services.AddMvc()
        .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
          options.LoginPath = E.Setting.Login;
        });
    }

    public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
}
