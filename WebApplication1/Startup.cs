using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Data;
using WebApplication1.Services;
using MongoDB.Driver;
using SDHCC.Identity;
using SDHCC.DB;

namespace WebApplication1
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //services.AddDbContext<ApplicationDbContext>(options =>
      //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      services.AddSingleton<IMongoDatabase>(s =>
      {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("lalala");
        return database;
      });
      //services.AddScoped<IUserStore<MUser>>(provider =>
      //      {
      //        var client = new MongoClient("mongodb://localhost:27017");
      //        var database = client.GetDatabase("lalala");

      //        return new MUserStore<MUser>(database);
      //      });
      services.AddScoped<ISDHCCDbContext, SDHCCDbContext>();
      services.AddScoped<IRoleStore<MRole>, SDHCCRoleStore<MRole>>();
      services.AddScoped<IUserRoleStore<MUser>, SDHCCUserRoleStore<MUser, MRole, MUserRole>>();
      services.AddScoped<IUserStore<MUser>, SDHCCUserStore<MUser>>();
      

      services.AddScoped<UserManager<MUser>>();
      services.AddScoped<RoleManager<MRole>>();
      services.AddIdentity<MUser, MRole>(option =>
      {

      }).AddDefaultTokenProviders();

      services.AddMvc()
          .AddRazorPagesOptions(options =>
          {
            options.Conventions.AuthorizeFolder("/Account/Manage");
            options.Conventions.AuthorizePage("/Account/Logout");
          });

      // Register no-op EmailSender used by account confirmation and password reset during development
      // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
      services.AddSingleton<IEmailSender, EmailSender>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
      }

      app.UseStaticFiles();

      app.UseAuthentication();

      app.UseMvc();
    }
  }
}
