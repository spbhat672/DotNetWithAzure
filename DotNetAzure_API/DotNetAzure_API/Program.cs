using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DotNetAzure_API.Data;
using DotNetAzure_API.Models.Messaging;
using DotNetAzure_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = builder.Configuration["KeyVaultURI"]; Console.WriteLine($"{keyVaultUri}");

if (string.IsNullOrEmpty(keyVaultUri))

    throw new ArgumentNullException("KeyVaultUri is not regisered");
builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());

string serviceBusNamespace =

    builder.Configuration["ServiceBus:FullyQualifiedNamespace"]

    ?? throw new InvalidOperationException("Namespace is missing.");

builder.Services.AddSingleton(_ =>

    new ServiceBusClient(

        serviceBusNamespace,

        new DefaultAzureCredential(),

        new ServiceBusClientOptions
        {

            TransportType = ServiceBusTransportType.AmqpWebSockets

        }));

string queueName = builder.Configuration["ServiceBus:QueueName"];


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services
    .AddAuthentication(
        Microsoft.AspNetCore.Authentication.JwtBearer
            .JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(
        builder.Configuration.GetSection("AzureAd")); builder.Services.AddAuthorization();


var cstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<TaskRedisCache>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddSingleton(new TaskQueueSetting(queueName));
builder.Services.AddSingleton<TaskItemSender>();
//builder.Services.AddHostedService<TaskItemReceiver>();

builder.Services.AddHttpClient<TaskApiClient>(client =>
{
    var baseUrl = builder.Configuration[
        "TaskItemApi:BaseUrl"]
        ?? throw new InvalidOperationException(
            "Order API BaseUrl is missing.");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});


//builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .WithOrigins("https://localhost:7194")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(options =>

{

// Define JWT Bearer authentication
options.AddSecurityDefinition("Bearer",

new Microsoft.OpenApi.Models.OpenApiSecurityScheme
{

    Name = "Authorization",

    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,

    Scheme = "bearer",

    BearerFormat = "JWT",

    In = Microsoft.OpenApi.Models.ParameterLocation.Header,

    Description = "Enter your JWT access token"
});


// Apply JWT authentication to Swagger endpoints
options.AddSecurityRequirement(

new Microsoft.OpenApi.Models.OpenApiSecurityRequirement        {

            {

                new Microsoft.OpenApi.Models.OpenApiSecurityScheme                {

                    Reference = new Microsoft.OpenApi.Models.OpenApiReference                    {

                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,

                        Id = "Bearer"                    }

                },

                Array.Empty<string>()

            }

        });

});

var app = builder.Build();

app.UseCors("ReactPolicy");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();