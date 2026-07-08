using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Domain.Repositories
{
    public interface ICommentRepository : IRepository<Comment, Guid>
    {
        Task<IList<Comment>> GetByPostIdAsync(Guid postId);
        Task<int> GetCommentCountByPostIdAsync(Guid postId);
        Task<(IList<Comment>, int total, int totalDisplay)> GetPagedBlogCommentsAsync(int pageIndex, int pageSize, string? searchText, string? sortOrder, Guid id);
    }
}
