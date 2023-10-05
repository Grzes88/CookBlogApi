using CookBlog.App.DTO;
using CookBlog.App.Services;
using Microsoft.AspNetCore.Components;

namespace CookBlog.App.Pages.Posts;

public partial class PostDetail
{
    [Parameter]
    public string PostId { get; set; }
    public PostDto PostDto { get; set; } = new PostDto();
    public CommentDto CommentDto { get; set; } = new CommentDto();
    public CreateCommentDto CreateCommentDto { get; set; } = new CreateCommentDto();

    [Inject]
    public IPostDataService PostDataService { get; set; }
    [Inject]
    public ICommentDataService CommentDataService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PostDto = await PostDataService.GetPostAsync(Guid.Parse(PostId));
    }

    public async Task AddComment()
    {
        var createCommentDto = new CreateCommentDto
        {
            PostId = Guid.Parse(PostId),
            FullName = CommentDto.FullName,
            Description = CommentDto.Description
        };
        await CommentDataService.AddCommentAsync(createCommentDto);

        CommentDto = new CommentDto();
    }
}
