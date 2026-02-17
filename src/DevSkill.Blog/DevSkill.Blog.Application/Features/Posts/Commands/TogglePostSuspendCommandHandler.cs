using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class TogglePostSuspendCommandHandler : ICommandHandler<TogglePostSuspendCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public TogglePostSuspendCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(TogglePostSuspendCommand command, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(command.Id);
            if (post == null)
            {
                throw new Exception("Post not found");
            }
            post.IsSuspended = command.IsSuspended;
            await _unitOfWork.PostRepository.EditAsync(post);
            await _unitOfWork.SaveAsync();
            return post.Id;
        }
    }
}
