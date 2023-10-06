using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using CookBlog.Api.Core.Exceptions;
using CookBlog.Api.Core.ValuesObjects;
using CookBlog.Core.Abstractions;
using CookBlog.Core.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using CookBlog.Core.Exceptions;

namespace CookBlog.Infrastructure.DAL.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly AzureOptions _azureOptions;
    private readonly ExtensionFileOptions _extensionFileOptions;
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(IOptions<AzureOptions> azureOptions,
        ExtensionFileOptions extensionFileOptions, BlobServiceClient blobServiceClient)
    {
        _azureOptions = azureOptions.Value;
        _extensionFileOptions = extensionFileOptions;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<FileDto> GetBlobImageAsync(ImagePath image)
    {
        if (image == null)
        {
            throw new InvalidImagePathException(image);
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient(_azureOptions.Container);
        var blobClient = containerClient.GetBlobClient(image);

        BlobDownloadResult content = await blobClient.DownloadContentAsync();
        var bytes = content.Content.ToArray();

        return new FileDto(bytes, image, content.Details.ContentType);
    }

    public async Task<ImagePath> UploadBlobImageAsync(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0)
            throw new InvalidFileException(formFile.FileName);

        var fileExtension = Path.GetExtension(formFile.FileName);
        if (!_extensionFileOptions.AllowedExtensions.Contains(fileExtension))
            throw new InvalidFileExtensionsException(fileExtension);

        using MemoryStream fileUploadStream = new MemoryStream();
        formFile.CopyTo(fileUploadStream);
        fileUploadStream.Position = 0;

        var uniqueName = $"{Guid.NewGuid()}{fileExtension}";

        var containerClient = _blobServiceClient.GetBlobContainerClient(_azureOptions.Container);
        var blobClient = containerClient.GetBlobClient(uniqueName);

        blobClient.Upload(fileUploadStream, new BlobUploadOptions()
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = formFile.ContentType
            }
        }, cancellationToken: default);

        return uniqueName;
    }
}
