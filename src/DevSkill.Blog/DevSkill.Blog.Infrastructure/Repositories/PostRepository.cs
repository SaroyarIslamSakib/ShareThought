using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Repositories;
using DevSkill.Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure.Repositories
{
    public class PostRepository : Repository<Post, Guid>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<(IList<Post>, int total, int totalDisplay)> GetPagedDraftPostsAsync
            (int pageIndex, int pageSize, string? searchText, string? sortOrder, Guid BlogId)
        {
            return await GetDynamicAsync(x => x.BlogAreaId == BlogId 
                && x.Title.Contains(searchText) 
                && x.IsPublished == false,
                sortOrder, 
                null, 
                pageIndex, 
                pageSize);
        }

        public async Task<(IList<Post>, int total, int totalDisplay)> GetPublishedPagedPostsAsync
            (int pageIndex, int pageSize, string? searchText, string? sortOrder, Guid BlogId)
        {
            return await GetDynamicAsync(x => x.BlogAreaId==BlogId && x.Title.Contains(searchText) 
                && x.IsPublished == true, sortOrder,
                q => q.Include(x => x.Comments.Where(m => m.IsDeleted == false))
                      .Include(x => x.BlogArea),
                pageIndex, 
                pageSize);

        }

        public async Task<(IList<Post>, int total, int totalDisplay)> GetPagedPublicPostsAsync
            (int pageIndex, int pageSize, string? searchText, string? sortOrder, string? categoryName)
        {
           if(categoryName == string.Empty || categoryName == null)
            {
                return await GetDynamicAsync(x => (x.Title.Contains(searchText)
                    || x.Tags.Any(t => t.Name.Contains(searchText)
                    || x.BlogArea.Name.Contains(searchText)))
                    && x.IsPublished == true, 
                    sortOrder,
                    q=>q.Include(x => x.Comments.Where(m => m.IsDeleted ==false)).Include(x => x.BlogArea).Include(y => y.Reports), 
                    pageIndex, 
                    pageSize);
            }
           else
            {
                return await GetDynamicAsync(x => (x.Title.Contains(searchText)
                || x.Tags.Any(t => t.Name.Contains(searchText)
                || x.BlogArea.Name.Contains(searchText)))
                && x.IsPublished == true 
                && x.PostCategories.Any(c => c.Name == categoryName), 
                sortOrder,
                q => q.Include(x => x.Comments.Where(m => m.IsDeleted == false)).Include(x => x.BlogArea).Include(y => y.Reports),
                pageIndex, 
                pageSize);
            }
        }

        public async Task<Post?> GetPostWithCategoriesTagsAsync(Guid postId)
        {
            var posts = await GetAsync(
                p => p.Id == postId,
                q => q
                    .Include(p => p.PostCategories)
                    .Include(p => p.Tags)
                    .Include(p => p.Reports)
            );

            return posts.FirstOrDefault();
        }
    }
}
