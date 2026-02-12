using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Comments.Commands
{
    public class MarkCommentAsApprovedCommandHandler : ICommandHandler<MarkCommentAsApprovedCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public MarkCommentAsApprovedCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(MarkCommentAsApprovedCommand command, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(command.Id);
            comment.IsApproved = true;
            await _unitOfWork.CommentRepository.EditAsync(comment);
            await _unitOfWork.SaveAsync();
            return command.Id;
        }
    }
}
