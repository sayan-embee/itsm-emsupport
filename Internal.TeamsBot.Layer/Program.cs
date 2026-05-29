using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.TeamsFx.Conversation;
using Microsoft.Bot.Builder;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.DbAccess;
using Microsoft.ApplicationInsights;
using Internal.TeamsBot.Layer;
using Internal.TeamsBot.Layer.Commands;
using Common.Layer.Models.AppSettings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using NSwag.Generation.Processors.Security;
using Internal.TeamsBot.Layer.Services.Swagger;
using Internal.TeamsBot.Layer.Bots;
using Internal.TeamsBot.Layer.Services.AdaptiveCards;
using Internal.TeamsBot.Layer.Services.SearchService;
using Internal.TeamsBot.Layer.Services.Notification;
using Internal.TeamsBot.Layer.Plugins;
using Microsoft.SemanticKernel;
using DataAccess.Layer.Data.FreshService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsModel>(builder.Configuration.GetSection("AppConfig"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettingsModel>>().Value);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
////builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
//builder.Services.AddHttpContextAccessor();

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

builder.Services.Configure<AISearchSettingsModel>(builder.Configuration.GetSection("AISearch"));
builder.Services.Configure<AzureOpenAISettingsModel>(builder.Configuration.GetSection("AzureOpenAI"));

builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Internal - Teams Bot";
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
        document.Info.Title = "Internal - Teams Bot";
        document.Info.Description = "Internal - Teams Bot in .Net 8.0";
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

// Create command handlers and the Conversation with command-response feature enabled.
builder.Services.AddSingleton<HelloWorldCommandHandler>();
builder.Services.AddSingleton(sp =>
{
    var options = new ConversationOptions()
    {
        Adapter = sp.GetService<CloudAdapter>(),
        //Command = new CommandOptions()
        //{
        //    Commands = new List<ITeamsCommandHandler> { sp.GetService<HelloWorldCommandHandler>() }
        //}
    };

    return new ConversationBot(options);
});

builder.Services.AddSingleton<IStorage, MemoryStorage>(); // Add MemoryStorage
builder.Services.AddSingleton<UserState>(); // UserState now has a storage implementation
builder.Services.AddSingleton<ConversationState>(); // ConversationState now has a storage implementation // Create the Conversation state. (Used by the Dialog system itself.)

builder.Services.AddSingleton<ShowTypingMiddleware>();

builder.Services.AddSingleton<TelemetryClient>();

builder.Services.AddSingleton<ISQLDataAccess, SQLDataAccess>();
builder.Services.AddScoped<ICommonData, CommonData>();
builder.Services.AddScoped<IFreshServiceData, FreshServiceData>();

builder.Services.AddTransient<IAppLifecycleHandler, AppLifecycleHandler>();
builder.Services.AddTransient<IBotConversationHandler, BotConversationHandler>();
builder.Services.AddTransient<IAdaptiveCardService, AdaptiveCardService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddTransient<IAISearch, AISearch>();
builder.Services.AddTransient<IBot, TeamsBot>();

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

//app.UseDefaultFiles()
//    .UseStaticFiles()
//    .UseWebSockets()
//    .UseRouting()
//    .UseAuthorization()
//    .UseEndpoints(endpoints =>
//    {
//        endpoints.MapControllers();
//    });

app.UseOpenApi();
app.UseSwaggerUi();

// Configure the HTTP request pipeline.
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseHttpsRedirection();

app.UseRouting();
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