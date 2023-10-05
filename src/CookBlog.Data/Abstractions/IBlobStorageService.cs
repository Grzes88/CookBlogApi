using CookBlog.Api.Core.ValuesObjects;
using CookBlog.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace CookBlog.Core.Abstractions;

public interface IBlobStorageService
{
    Task<ImagePath> UploadBlobImageAsync(IFormFile formFile);
    Task<FileDto> GetBlobImageAsync(ImagePath imageBlob);
}
