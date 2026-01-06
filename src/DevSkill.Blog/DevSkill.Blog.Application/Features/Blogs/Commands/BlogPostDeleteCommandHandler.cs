using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Commands
{
    public class BlogPostDeleteCommandHandler : ICommandHandler<BlogPostDeleteCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public BlogPostDeleteCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(BlogPostDeleteCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.BlogPostRepository.RemoveAsync(command.Id);
            await _unitOfWork.SaveAsync();
            return command.Id;
        }
    }
}
