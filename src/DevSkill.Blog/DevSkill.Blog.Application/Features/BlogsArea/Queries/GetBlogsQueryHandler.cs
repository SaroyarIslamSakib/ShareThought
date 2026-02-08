using Cortex.Mediator.Queries;
using DevSkill.Blog.Application.Features.Contacts.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.BlogsArea.Queries
{
    public class GetBlogsQueryHandler : IQueryHandler<GetBlogsQuery, (IList<BlogDto>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public GetBlogsQueryHandler(IApplicationUnitOfWork unitOfWork,IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<(IList<BlogDto>, int total, int totalDisplay)> Handle(GetBlogsQuery query, CancellationToken cancellationToken)
        {
            var blogs = await _unitOfWork.BlogAreaRepository.GetPagedBlogsAsync(query.PageIndex, query.PageSize, query.SearchText, query.SortOrder);
            var blogDtos = new List<BlogDto>();
            foreach(var blog in blogs.Item1)
            {
                var owner = await _userService.GetUserByIdAsync(blog.UserId);
                var blogDto = new BlogDto
                {
                    Id = blog.Id,
                    Title = blog.Name,
                    OwnerName = owner.FullName,
                    OwnerEmail = owner.Email,
                    TotalPosts = await _unitOfWork.PostRepository.GetCountAsync(x => x.BlogAreaId == blog.Id),
                    CreatedAt = blog.CreatedAt,
                    IsSuspended = blog.IsSuspended
                };
                blogDtos.Add(blogDto);

            }
            return (blogDtos, blogs.Item2, blogs.Item3);
        }
    }
}
