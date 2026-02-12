using DevSkill.Blog.Domain.Dtos;
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
    public class CommentRepository : Repository<Comment, Guid>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {

        }

        public Task<IList<Comment>> GetByPostIdAsync(Guid postId)
        {
            return GetAsync(c => c.PostId == postId && c.IsApproved, null);
        }

        public async Task<int> GetCommentCountByPostIdAsync(Guid postId)
        {
            return GetCount(c => c.PostId == postId && !c.IsDeleted && c.IsApproved);
        }

        public async Task<(IList<Comment>, int total, int totalDisplay)> GetPagedBlogCommentsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder, Guid id)
        {
            return await GetDynamicAsync(x => x.Post.BlogAreaId == id && !x.IsDeleted && !x.IsApproved && (x.Content.Contains(searchText) || x.Post.Title.Contains(searchText)),
                sortOrder,
                q => q.Include(x => x.Post),
                pageIndex,
                pageSize);
        }
    }
    
}
