using Azure.Identity;
using Azure.Storage.Blobs;
using BlobStorage.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serviceUri = builder.Configuration[

    "AzureBlobStorage:ServiceUri"];

var containerName = builder.Configuration[

    "AzureBlobStorage:ContainerName"];

if (string.IsNullOrWhiteSpace(serviceUri) ||

    string.IsNullOrWhiteSpace(containerName))

{

    throw new InvalidOperationException(

        "Azure Blob Storage configuration is missing.");

}

// Select authentication based on environment// Register BlobServiceClient
builder.Services.AddSingleton(_ =>

new BlobServiceClient(

    new Uri(serviceUri),

    new AzureCliCredential()));

// Register BlobContainerClient
builder.Services.AddSingleton(provider =>

{

    var blobServiceClient =

        provider.GetRequiredService<BlobServiceClient>();


    return blobServiceClient.GetBlobContainerClient(

        containerName);

});

// Register BlobStorageService
builder.Services.AddScoped<BlobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
