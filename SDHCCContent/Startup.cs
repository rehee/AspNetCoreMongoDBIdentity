using System;
using System.Buffers;
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
using SDHCC;
using SDHCC.DB;
using SDHCC.DB.Content;
using SDHCC.DB.Models;
using SDHCC.Identity;
using SDHCC.Identity.Models.UserRoles;
using SDHCC.Identity.Services;
using SDHCCContent.Controllers;
using SDHCCContent.Models.DropDowns;

namespace SDHCCContent
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
    {
      Configuration = configuration;
      HostingEnvironment = hostingEnvironment;
    }

    public IConfiguration Configuration { get; }
    public IHostingEnvironment HostingEnvironment { get; }
    public void ConfigureServices(IServiceCollection services)
    {
      StartUpFunction.ConfigureServices<TestUser, SDHCCContent.Models.ContentBaseModel, DropDownBaseModel>(
        services, Configuration, HostingEnvironment);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      StartUpFunction.Configure(app, env);
      
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
