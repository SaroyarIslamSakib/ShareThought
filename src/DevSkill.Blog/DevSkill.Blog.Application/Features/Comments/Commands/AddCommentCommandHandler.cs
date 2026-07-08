using Cortex.Mediator.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;

namespace DevSkill.Blog.Application.Features.Comments.Commands
{
    public class AddCommentCommandHandler : ICommandHandler<AddCommentCommand, CommentDto>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IServerTime _serverTime;
        private readonly IUserService _userService;
        public AddCommentCommandHandler(IApplicationUnitOfWork unitOfWork, IServerTime serverTime, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _serverTime = serverTime;
            _userService = userService;
        }

        public async Task<CommentDto> Handle(AddCommentCommand command, CancellationToken cancellationToken)
        {
            var comment = new Comment
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                PostId = command.PostId,
                ParentId = command.ParentId,
                UserId = command.UserId,
                Content = command.Content,
                CreatedAt = _serverTime.DateTime,
                UpvoteCount = 0
            };

            await _unitOfWork.CommentRepository.AddAsync(comment);
            await _unitOfWork.SaveAsync();

            var user = await _userService.GetUserByIdAsync(command.UserId);
            return new CommentDto
            {
                id = comment.Id,
                parent = comment.ParentId,
                content = comment.Content,
                created = comment.CreatedAt,
                fullname = user.FullName,
                upvote_count = 0,
                user_has_upvoted = false
            };
        }
    }
}
