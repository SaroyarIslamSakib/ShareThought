using Cortex.Mediator.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using MapsterMapper;

namespace DevSkill.Blog.Application.Features.BlogsArea.Commands
{
    public class AddBlogAreaCommandHandler : ICommandHandler<AddBlogAreaCommand, BlogArea>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISlugService _slugService;
        public AddBlogAreaCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper,ISlugService slugService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slugService = slugService;
        }
        public async Task<BlogArea> Handle(AddBlogAreaCommand command, CancellationToken cancellationToken)
        {
            var blog = _mapper.Map<BlogArea>(command);
            blog.Id = IdentityGenerator.NewSequentialGuid();
            blog.Slug =await _slugService.GenerateUniqueSlugAsync(command.Name);
            await _unitOfWork.BlogAreaRepository.AddAsync(blog);
            await _unitOfWork.SaveAsync();
            return blog;
        }
    }
}
