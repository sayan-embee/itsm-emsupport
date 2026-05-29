using Common.Layer.Models.AppSettings;
using External.CustomerPortal.Layer;
using External.CustomerPortal.Layer.Bot;
using External.CustomerPortal.Layer.Services.Swagger;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Options;
using NSwag.Generation.Processors.Security;
using Microsoft.ApplicationInsights;
using DataAccess.Layer.DbAccess;
using DataAccess.Layer.Data.Common;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Common.Layer.Models.JWT;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using External.CustomerPortal.Layer.Helpers.SMTP;
using External.CustomerPortal.Layer.Services.SMTP;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.Services.JWT;
using Common.Layer.Models.WebChatBot;
using External.CustomerPortal.Layer.Services.WebChatBot;
using External.CustomerPortal.Layer.Services.AdaptiveCards;
using Microsoft.SemanticKernel;
using External.CustomerPortal.Layer.Plugins;
using Microsoft.Extensions.Caching.Memory;
using External.CustomerPortal.Layer.Services.GraphAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsModel>(builder.Configuration.GetSection("AppConfig"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettingsModel>>().Value);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
            "http://localhost:5131", // Client 1
            "https://9h0ht54t-5131.inc1.devtunnels.ms" // Client 2
        )
        .AllowAnyHeader() // Allow any header
        .AllowAnyMethod() // Allow any method (GET, POST, etc.)
        .AllowCredentials(); // Allow credentials (cookies)
    });
});

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "env/.env"));
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSettings>>().Value);

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
string[] audience = jwtSettings.GetSection("Audience").Get<string[]>();

builder.Configuration["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSettings["SecretKey"];
secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSettings["SecretKey"];

Console.WriteLine($"Configured Audiences: {string.Join(", ", audience)}");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,

        ValidateAudience = true,
        ValidAudiences = audience,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Use token from cookie ONLY if Authorization header is missing
            if (string.IsNullOrEmpty(context.Token))
            {
                context.Token = context.Request.Cookies["jwtToken"];
            }
            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            context.Response.Headers.Append("Token-Expired", "true");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.Configure<AISearchSettingsModel>(builder.Configuration.GetSection("AISearch"));
builder.Services.Configure<AzureOpenAISettingsModel>(builder.Configuration.GetSection("AzureOpenAI"));


builder.Services.Configure<WebChatSettings>(builder.Configuration.GetSection("WebChatSettings"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<WebChatSettings>>().Value);

builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "External - Customer Portal";
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer")); // Add bearer token
    config.OperationProcessors.Add(new CustomHeaderOperationProcessor()); // Add custom header processor
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
        document.Info.Title = "External - Customer Portal";
        document.Info.Description = "External - Customer Portal in .Net 8.0";
    };
});

// Prepare Configuration for ConfigurationBotFrameworkAuthentication
var config = builder.Configuration.Get<ConfigOptions>();
builder.Configuration["MicrosoftAppType"] = "MultiTenant";
builder.Configuration["MicrosoftAppId"] = config.BOT_ID;
builder.Configuration["MicrosoftAppPassword"] = config.BOT_PASSWORD;

builder.Services.Configure<ConfigOptions>(builder.Configuration);
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ConfigOptions>>().Value);

// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

// Create the Cloud Adapter with error handling enabled.
// Note: some classes expect a BotAdapter and some expect a BotFrameworkHttpAdapter, so
// register the same adapter instance for both types.
builder.Services.AddSingleton<CloudAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<CloudAdapter>());
builder.Services.AddSingleton<BotAdapter>(sp => sp.GetService<CloudAdapter>());

builder.Services.AddSingleton<IStorage, MemoryStorage>(); // Add MemoryStorage
builder.Services.AddSingleton<UserState>(); // UserState now has a storage implementation
builder.Services.AddSingleton<ConversationState>(); // ConversationState now has a storage implementation // Create the Conversation state. (Used by the Dialog system itself.)

builder.Services.AddSingleton<ShowTypingMiddleware>();

builder.Services.AddSingleton<TelemetryClient>();

builder.Services.AddSingleton<ISQLDataAccess, SQLDataAccess>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

builder.Services.AddScoped<ICommonData, CommonData>();
builder.Services.AddScoped<ICustomerPortalData, CustomerPortalData>();

builder.Services.AddScoped<SupportDialog>();

builder.Services.AddTransient<IBotConversationHandler, BotConversationHandler>();

builder.Services.AddTransient<IGraphAPIService, GraphAPIService>();
builder.Services.AddTransient<ISmtpHelper, SmtpHelper>();
builder.Services.AddTransient<ISmtpService, SmtpService>();

builder.Services.AddTransient<IWebChatBotService, WebChatBotService>();

builder.Services.AddTransient<IAdaptiveCardService, AdaptiveCardService>();
builder.Services.AddTransient<IBot, EchoBot>();

builder.Services.AddHostedService<IdleUserMonitorService>();

var azureOpenAISettings = builder.Configuration.GetSection("AzureOpenAI");
var deploymentId = azureOpenAISettings["DeploymentId"];
var endPoint = azureOpenAISettings["EndPoint"];
var apiKey = azureOpenAISettings["ApiKey"];
var deploymentIdTextEmbeddings = azureOpenAISettings["DeploymentIdTextEmbeddings"];

builder.Services.AddSingleton<SOPSearchPlugin>();
//builder.Services.AddSingleton<MSWebSearchPlugin>();
//builder.Services.AddSingleton<TicketPlugin>();

builder.Services.AddSingleton<Kernel>(serviceProvider =>
{
    Console.WriteLine("Starting Kernel Initialization...");

    var kernelBuilder = Kernel.CreateBuilder();

    Console.WriteLine("Adding Azure OpenAI Chat Completion...");
    kernelBuilder.AddAzureOpenAIChatCompletion(
        deploymentName: deploymentId ?? throw new InvalidOperationException("Azure OpenAI Deployment Name is not set."),
        modelId: deploymentId ?? throw new InvalidOperationException("Azure OpenAI Model ID is not set."),
        endpoint: endPoint ?? throw new InvalidOperationException("Azure OpenAI API Endpoint is not set."),
        apiKey: apiKey ?? throw new InvalidOperationException("Azure OpenAI API Key is not set.")
    );

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    Console.WriteLine("Adding Azure OpenAI Text Embedding Generation...");
    kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
       deploymentName: deploymentIdTextEmbeddings,
       modelId: deploymentIdTextEmbeddings,
       endpoint: endPoint,
       apiKey: apiKey
       );

    Console.WriteLine("Registering Plugins...");

    //kernelBuilder.Plugins.AddFromType<SOPSearchPlugin>();
    //kernelBuilder.Plugins.AddFromType<MSWebSearchPlugin>();
    //kernelBuilder.Plugins.AddFromType<TicketPlugin>();

    var sopSearchPlugin = serviceProvider.GetRequiredService<SOPSearchPlugin>();
    //var msWebSearchPlugin = serviceProvider.GetRequiredService<MSWebSearchPlugin>();
    //var ticketSearchPlugin = serviceProvider.GetRequiredService<TicketPlugin>();

    kernelBuilder.Plugins.AddFromObject(sopSearchPlugin);
    //kernelBuilder.Plugins.AddFromObject(msWebSearchPlugin);
    //kernelBuilder.Plugins.AddFromObject(ticketSearchPlugin);

    var kernel = kernelBuilder.Build();

    Console.WriteLine("Kernel Initialization Completed Successfully!");

    return kernel;
});

builder.Services.AddMemoryCache();

builder.Services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/build");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseOpenApi();
app.UseSwaggerUi();

// Configure the HTTP request pipeline.
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();

app.Use(async (context, next) =>
{
    string clientOrigin = context.Request.Headers["Origin"].ToString();
    if (string.IsNullOrEmpty(clientOrigin))
    {
        clientOrigin = context.Request.Headers["Referer"].ToString();
    }

    context.Items["ClientOrigin"] = clientOrigin;

    await next();
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseMvc();
app.MapControllers();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();