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

namespace DevSkill.Blog.Application.Features.BlogsArea.Commands
{
    public class AddBlogAreaCommandHandler : ICommandHandler<AddBlogAreaCommand, BlogArea>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddBlogAreaCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<BlogArea> Handle(AddBlogAreaCommand command, CancellationToken cancellationToken)
        {
            var blog = _mapper.Map<BlogArea>(command);
            blog.Id = IdentityGenerator.NewSequentialGuid();
            await _unitOfWork.BlogAreaRepository.AddAsync(blog);
            await _unitOfWork.SaveAsync();
            return blog;


        }
    }
}
