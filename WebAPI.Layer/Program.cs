using Microsoft.ApplicationInsights;
using DataAccess.Layer.Data;
using DataAccess.Layer.DbAccess;
using Common.Layer.Models.AppSettings;
using Microsoft.Extensions.Options;
using DataAccess.Layer.Data.Site24x7;
using DataAccess.Layer.Data.FreshService;
using WebAPI.Layer.Services;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using System.Text.Json;
using NSwag.Generation.Processors.Security;
using WebAPI.Layer.Services.Swagger;
using DataAccess.Layer.Data.Common;
using WebAPI.Layer.Services.SMTP;
using WebAPI.Layer.Helpers.Files;
using WebAPI.Layer.Helpers;
using DataAccess.Layer.Data.CustomerPortal;
using WebAPI.Layer.Helpers.SMTP;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common.Layer.Models.JWT;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsModel>(builder.Configuration.GetSection("AppConfig"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettingsModel>>().Value);

builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddAuthorization();

builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "WebAPIApplication";
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
    config.OperationProcessors.Add(new CustomHeaderOperationProcessor());
    config.AddSecurity("Bearer", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    config.PostProcess = document =>
    {
        document.Info.Version = "v1";
        document.Info.Title = "WebAPIApplication";
        document.Info.Description = "WebAPIApplication in .Net 8.0";
    };
});

builder.Services.AddSingleton<TelemetryClient>();

//Adding Services
builder.Services.AddSingleton<ISQLDataAccess, SQLDataAccess>();

builder.Services.AddScoped<ISite24x7Data, Site24x7Data>();
builder.Services.AddScoped<IFreshServiceData, FreshServiceData>();
builder.Services.AddScoped<ICommonData, CommonData>();
builder.Services.AddScoped<ICustomerPortalData, CustomerPortalData>();

builder.Services.AddScoped<IGenPPTService, GenPPTService>();
builder.Services.AddScoped<IGenExcelService, GenExcelService>();

builder.Services.AddTransient<IFileHelper, FileHelper>();
builder.Services.AddTransient<ISmtpHelper, SmtpHelper>();
builder.Services.AddTransient<ISmtpService, SmtpService>();

builder.Services.AddMemoryCache();

//builder.Services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/build");

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseOpenApi();
//    app.UseSwaggerUi();
//}

app.UseOpenApi();
app.UseSwaggerUi();

// Configure the HTTP request pipeline.
app.UseDefaultFiles();
app.UseStaticFiles();
//app.UseSpaStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMvc();
app.MapControllers();
//app.UseSpa(spa =>
//{
//    spa.Options.SourcePath = "ClientApp";

//    if (app.Environment.IsDevelopment())
//    {
//        spa.UseReactDevelopmentServer(npmScript: "start");
//    }
//});

app.Run();
