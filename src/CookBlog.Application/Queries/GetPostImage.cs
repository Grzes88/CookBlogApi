using CookBlog.Api.Application.Abstractions;
using CookBlog.Api.Core.ValuesObjects;
using CookBlog.Core.DTO;

namespace CookBlog.Application.Queries
{
    public sealed record GetPostImage(PostId PostId) : IQuery<FileDto>;
}