using CookBlog.Api.Application.Abstractions;
using CookBlog.Api.Core.Exceptions;
using CookBlog.Api.Core.Repositories;
using CookBlog.Application.Queries;
using CookBlog.Core.Abstractions;
using CookBlog.Core.DTO;

namespace CookBlog.Infrastructure.DAL.Handlers;

public class GetPostImageHandler : IQueryHandler<GetPostImage, FileDto>
{
    private readonly IPostRepository _postRepository;
    private readonly IBlobStorageService _blobStorageService;

    public GetPostImageHandler(IPostRepository postRepository,
        IBlobStorageService blobStorageService)
    {
        _postRepository = postRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<FileDto> HandleAsync(GetPostImage query)
    {
        var image = await _postRepository.GetImagePathAsync(query.PostId);
        if (image is null)
        {
            throw new InvalidImagePathException(image);
        }

        var fileDto = await _blobStorageService.GetBlobImageAsync(image);

        return fileDto;
    }
}