using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Commands
{
    public class BlogPostEditCommandHandler : ICommandHandler<BlogPostEditCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BlogPostEditCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Guid> Handle(BlogPostEditCommand command, CancellationToken cancellationToken)
        {
            var blog = _mapper.Map<BlogPost>(command);
            await _unitOfWork.BlogPostRepository.EditAsync(blog);
            await _unitOfWork.SaveAsync();
            return blog.Id;
        }
    }
}
