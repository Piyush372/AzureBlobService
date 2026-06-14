using System.Threading.Tasks;
using Azure.Storage.Blobs;
using System.Collections.Generic;

public interface IContainerService
{
    Task<string> CreateContainerAsync(string containerName);
    Task DeleteContainerAsync(string containerName);
    Task<bool> ContainerExistsAsync(string containerName);
    Task<string[]> ListContainersAsync();
}

public class ContainerService : IContainerService
{
    private readonly BlobServiceClient _blobServiceClient;

    public ContainerService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> CreateContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient.Uri.ToString();
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.DeleteIfExistsAsync();
    }

    public async Task<bool> ContainerExistsAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return await containerClient.ExistsAsync();
    }

    public async Task<string[]> ListContainersAsync()
    {
        var containers = new List<string>();
        await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
        {
            containers.Add(container.Name);
        }
        return containers.ToArray();
    }
}