using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;


namespace AzureCloudServices.Endpoints;


public static class ContainerEndpoints
{
    public static IEndpointRouteBuilder MapContainerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var v1Group  = endpoints.NewVersionedApi("ContainerEndpoints");
        var containerEndpointsGroup = v1Group.MapGroup("/api/v{version:apiVersion}/container-service")
                                    .HasApiVersion(1, 0);

        // Create container endpoint
        containerEndpointsGroup.MapPost("create", async (string containerName, IContainerService containerService) =>
        {
            var uri = await containerService.CreateContainerAsync(containerName);
            return Results.Ok($"Container '{containerName}' created successfully. URI: {uri}");
        })
        .WithName("CreateContainer")
        .WithSummary("Create a new blob storage container")
        .WithDescription("Creates a new blob storage container with the specified name.")
        .Produces(200);

        // Delete container endpoint
        containerEndpointsGroup.MapDelete("delete", async (string containerName, IContainerService containerService) =>
        {
            await containerService.DeleteContainerAsync(containerName);
            return Results.Ok($"Container '{containerName}' deleted successfully.");
        })
        .WithName("DeleteContainer")
        .WithSummary("Delete a blob storage container")
        .WithDescription("Deletes the specified blob storage container.")
        .Produces(200);

        // Check if container exists endpoint
        containerEndpointsGroup.MapGet("exists", async (string containerName, IContainerService containerService) =>
        {
            var exists = await containerService.ContainerExistsAsync(containerName);
            return Results.Ok(exists);
        })
        .WithName("CheckContainerExists")
        .WithSummary("Check if a blob storage container exists")
        .WithDescription("Checks if the specified blob storage container exists.")
        .Produces(200);

        // List containers endpoint
        containerEndpointsGroup.MapGet("list", async (IContainerService containerService) =>
        {
            var containers = await containerService.ListContainersAsync();
            return Results.Ok(containers);
        })
        .WithName("ListContainers")
        .WithSummary("List all blob storage containers")
        .WithDescription("Lists all blob storage containers in the Azure Blob Storage account.")
        .Produces(200);

        return endpoints;
    }
}