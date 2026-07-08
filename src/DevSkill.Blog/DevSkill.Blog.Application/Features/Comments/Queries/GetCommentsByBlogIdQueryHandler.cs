using Cortex.Mediator.Queries;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;

namespace DevSkill.Blog.Application.Features.Comments.Queries
{
    public class GetCommentsByBlogIdQueryHandler : IQueryHandler<GetCommentsByBlogIdQuery, (IList<BlogCommentDto>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public GetCommentsByBlogIdQueryHandler(IApplicationUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        public async Task<(IList<BlogCommentDto>, int total, int totalDisplay)> Handle(GetCommentsByBlogIdQuery query, CancellationToken cancellationToken)
        {
            var blog = (await _unitOfWork.BlogAreaRepository
                .GetByUserIdAsync(query.UserId))
                .FirstOrDefault();

            if (blog == null)
                throw new Exception("Blog not found.");

            var comments = await _unitOfWork.CommentRepository.GetPagedBlogCommentsAsync(
                query.PageIndex,
                query.PageSize,
                query.SearchText,
                query.SortOrder,
                blog.Id);

            var commentDtos = new List<BlogCommentDto>();

            foreach (var comment in comments.Item1)
            {
                var owner = await _userService.GetUserByIdAsync(blog.UserId);
                var commentDto = new BlogCommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    Created = comment.CreatedAt,
                    Parent = comment.ParentId,
                    UserName = owner.FullName,
                    PostTitle = comment.Post.Title,
                    IsApproved = comment.IsApproved,
                };
                commentDtos.Add(commentDto);

            }
            return (commentDtos, comments.Item2, comments.Item3);
        }
    }
}
