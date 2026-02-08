using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.BlogsArea.Commands
{
    public class ToggleBlogSuspendCommandHandler : ICommandHandler<ToggleBlogSuspendCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public ToggleBlogSuspendCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(ToggleBlogSuspendCommand command, CancellationToken cancellationToken)
        {
            var blog = await _unitOfWork.BlogAreaRepository.GetByIdAsync(command.Id);
            if (blog == null)
            {
                throw new Exception("Blog not found");
            }
            blog.IsSuspended = command.IsSuspended;
            await _unitOfWork.BlogAreaRepository.EditAsync(blog);
            await _unitOfWork.SaveAsync();
            return blog.Id;
        }
    }
}
