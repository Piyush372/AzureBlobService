using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using AzureCloudServices.Services;

namespace AzureCloudServices.Endpoints;

public static class BlobEndpoints
{
    public static  IEndpointRouteBuilder MapBlobEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var v1Group  = endpoints.NewVersionedApi("BlobEndpoints");
        var blobEndpointsGroup = v1Group.MapGroup("/api/v{version:apiVersion}/blob-service")
                                    .HasApiVersion(1, 0);
        
        // Upload endpoint
        blobEndpointsGroup.MapPost("upload", async (IFormFile file, IBlobStorageService blobStorageService) =>
        {
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest("No file uploaded.");
            }

            using var stream = file.OpenReadStream();
            await blobStorageService.UploadFileAsync("my-container", file.FileName, stream);
            return Results.Ok("File uploaded successfully.");
        })
        .DisableAntiforgery()
        .WithName("UploadFile")
        .WithSummary("Upload a file to blob storage")
        .WithDescription("Uploads the provided file to the 'my-container' Azure Blob Storage container.")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(200)
        .Produces(400);

        // Download endpoint
        blobEndpointsGroup.MapGet("download", async (string fileName, IBlobStorageService blobStorageService) =>
        {
            var stream = await blobStorageService.DownloadFileAsync("my-container", fileName);
            if (stream == null)
            {
                return Results.NotFound("File not found.");
            }

            return Results.File(stream, "application/octet-stream", fileName);
        })
        .WithName("DownloadFile")
        .WithSummary("Download a file from blob storage")
        .WithDescription("Downloads the specified file from the 'my-container' Azure Blob Storage container.")
        .Produces(200)
        .Produces(404);

        // Delete endpoint
        blobEndpointsGroup.MapDelete("delete", async (string fileName, IBlobStorageService blobStorageService) =>
        {
            await blobStorageService.DeleteFileAsync("my-container", fileName);
            return Results.Ok("File deleted successfully.");
        })
        .WithName("DeleteFile")
        .WithSummary("Delete a file from blob storage")
        .WithDescription("Deletes the specified file from the 'my-container' Azure Blob Storage container.")
        .Produces(200);


        return endpoints;
    }

}