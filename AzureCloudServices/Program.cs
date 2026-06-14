using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AzureCloudServices.Services;
using Microsoft.OpenApi;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AzureCloudServices.Endpoints;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton(s => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));
        builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
        builder.Services.AddScoped<IContainerService, ContainerService>();
        //builder.Services.AddAntiforgery();
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Azure Blob Storage API",
                Version = "v1",
                Description = "API for managing files in Azure Blob Storage",
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"Azure Blob Storage API {description.GroupName.ToUpperInvariant()}");
                }
                options.RoutePrefix = "swagger";
            });
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        //app.UseAntiforgery();

        app.MapBlobEndpoints();
        
        app.MapContainerEndpoints();
        app.Run();
    }

}
