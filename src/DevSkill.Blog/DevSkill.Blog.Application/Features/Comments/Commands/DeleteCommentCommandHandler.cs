using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;

namespace DevSkill.Blog.Application.Features.Comments.Commands
{
    public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public DeleteCommentCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.CommentRepository
            .GetByIdAsync(command.CommentId);

            if (comment == null)
                throw new Exception("Comment not found");

            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync();
            return comment.Id;
        }
    }
}
