using Cortex.Mediator.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;
using DevSkill.Blog.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Comments.Commands
{
    public class EditCommentCommandHandler : ICommandHandler<EditCommentCommand, CommentDto>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IServerTime _serverTime;

        public EditCommentCommandHandler(
            IApplicationUnitOfWork unitOfWork,
            IUserService userService,
            IServerTime serverTime)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _serverTime = serverTime;
        }
        public async Task<CommentDto> Handle(EditCommentCommand command, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.CommentRepository
             .GetByIdAsync(command.CommentId);

            if (comment == null)
                throw new Exception("Comment not found");

            if (comment.UserId != command.UserId)
                throw new UnauthorizedAccessException();

            comment.Content = command.Content;
            comment.UpdatedAt = _serverTime.DateTime;

            await _unitOfWork.CommentRepository.EditAsync(comment);
            await _unitOfWork.SaveAsync();

            var user = await _userService.GetUserByIdAsync(comment.UserId);

            return new CommentDto
            {
                id = comment.Id,
                parent = comment.ParentId,
                content = comment.Content,
                created = comment.CreatedAt,
                fullname = user.FullName,
                upvote_count = comment.UpvoteCount,
                user_has_upvoted = false,
                user_id = comment.UserId.ToString(),
                modified = comment.UpdatedAt

            };
        }
    }
}
