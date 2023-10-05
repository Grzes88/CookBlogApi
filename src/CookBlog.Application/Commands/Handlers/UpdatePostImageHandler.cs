using CookBlog.Api.Application.Abstractions;
using CookBlog.Api.Application.Exceptions;
using CookBlog.Api.Core.Repositories;
using CookBlog.Api.Core.ValuesObjects;
using CookBlog.Core.Abstractions;

namespace CookBlog.Application.Commands.Handlers;

public class UpdatePostImageHandler : ICommandHandler<UpdatePostImage>
{
    private readonly IPostRepository _postRepository;
    private readonly IBlobStorageService _blobStorageService;

    public UpdatePostImageHandler(IPostRepository postRepository,
        IBlobStorageService blobStorageService)
    {
        _postRepository = postRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task HandleAsync(UpdatePostImage command)
    {
        var postId = new PostId(command.PostId);
        var post = await _postRepository.GetAsync(postId);

        if (post == null)
        {
            throw new NotFoundPostException(postId);
        }

        var image = await _blobStorageService.UploadBlobImageAsync(command.FormFile);

        post.ChangeImage(image);
    }
}
