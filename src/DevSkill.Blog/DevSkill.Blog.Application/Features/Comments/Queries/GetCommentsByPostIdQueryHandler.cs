using Cortex.Mediator.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Comments.Queries
{
    public class GetCommentsByPostIdQueryHandler : IQueryHandler<GetCommentsByPostIdQuery, IList<CommentDto>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public GetCommentsByPostIdQueryHandler(
            IApplicationUnitOfWork unitOfWork,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<IList<CommentDto>> Handle(GetCommentsByPostIdQuery query, CancellationToken cancellationToken)
        {
            var comments = await _unitOfWork.CommentRepository.GetByPostIdAsync(query.PostId);

            var userIds = comments.Select(c => c.UserId).Distinct();

            var users = new List<UserListDto>();
            foreach (var userId in userIds)
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            return comments.Select(c =>
            {
                var user = users.First(u => u.Id == c.UserId);

                return new CommentDto
                {
                    id = c.Id,
                    parent = c.ParentId,
                    content = c.Content,
                    created = c.CreatedAt,
                    fullname = user.FullName,
                    upvote_count = c.UpvoteCount,
                    user_has_upvoted = false
                };
            }).ToList();
        }
    }
}
