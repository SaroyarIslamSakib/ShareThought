using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Blogs.Commands
{
    public class BlogPostAddCommandHandler : ICommandHandler<BlogPostAddCommand, BlogPost>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BlogPostAddCommandHandler(IApplicationUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<BlogPost> Handle(BlogPostAddCommand command, CancellationToken cancellationToken)
        {
            BlogPost post = _mapper.Map<BlogPost>(command);
            post.Id = IdentityGenerator.NewSequentialGuid();
            await _unitOfWork.BlogPostRepository.AddAsync(post);
            await _unitOfWork.SaveAsync();
            return post;
        }
    }
}
