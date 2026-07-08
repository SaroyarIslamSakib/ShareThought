using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<(IList<Post>, int total, int totalDisplay)> GetPagedDraftPostsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder, Guid id);
        Task<(IList<Post>, int total, int totalDisplay)> GetPublishedPagedPostsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder, Guid BlogId);
        Task<(IList<Post>, int total, int totalDisplay)> GetPagedPublicPostsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder,string? categoryName);
        Task<Post> GetPostWithCategoriesTagsAsync(Guid postId);
        Task<int> TotalLikeCountInBlogAsync(Guid blogId);
        Task<int> TotalCommentCountInBlogAsync(Guid blogId);
        Task<bool> ExistsBySlugAsync(string slug);
        Task<Post> GetPostBySlugAsync(string blogSlug, string postSlug);
        Task<(IList<Post>, int total, int totalDisplay)> GetPagedAdminPostsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder);
        Task<(IList<Post>, int total, int totalDisplay)> GetPagedPostsInBlogBySlugAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder, string? categoryName, string? blogSlug);
    }
}
