using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class DeletePostCommandHandler : ICommandHandler<DeletePostCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public DeletePostCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(DeletePostCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.PostRepository.RemoveAsync(command.PostId);
            await _unitOfWork.SaveAsync();
            return command.PostId;
        }
    }
}
