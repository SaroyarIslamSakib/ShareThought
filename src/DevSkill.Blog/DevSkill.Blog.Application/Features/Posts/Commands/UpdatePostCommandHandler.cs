using Cortex.Mediator.Commands;
using DevSkill.Blog.Application.Services;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class UpdatePostCommandHandler
    : ICommandHandler<UpdatePostCommand, Post>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly ISlugService _slugService;

        public UpdatePostCommandHandler(IApplicationUnitOfWork unitOfWork, ISlugService slugService)
        {
            _unitOfWork = unitOfWork;
            _slugService = slugService;
        }

        public async Task<Post> Handle(
     UpdatePostCommand command,
     CancellationToken cancellationToken)
        {
            //  MUST eager load categories & tags
            var post = await _unitOfWork.PostRepository
                .GetPostWithCategoriesTagsAsync(command.PostId);

            if (post == null)
                throw new Exception("Post not found.");


               // BASIC FIELDS

            if (!string.Equals(post.Title, command.Title, StringComparison.OrdinalIgnoreCase))
            {
                post.Slug = await _slugService
                    .GenerateUniqueSlugAsync(command.Title);
            }
            post.Title = command.Title;
            post.Content = command.Content;
            post.FeatureImagePath = command.FeatureImagePath;



              // CATEGORY UPDATE 

            post.PostCategories.Clear();

            var categoryNames = command.CategoryNames?
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
                ?? new List<string>();

            foreach (var name in categoryNames)
            {
                var category = await _unitOfWork
                    .CategoryRepository
                    .GetByNameAsync(name);

                if (category == null)
                {
                    category = new Category
                    {
                        Id = IdentityGenerator.NewSequentialGuid(),
                        Name = name
                    };

                    await _unitOfWork.CategoryRepository.AddAsync(category);
                }

                post.PostCategories.Add(category);
            }


              // TAG UPDATE 

            post.Tags.Clear();

            var tagNames = command.TagNames?
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
                ?? new List<string>();

            foreach (var name in tagNames)
            {
                var tag = await _unitOfWork
                    .TagRepository
                    .GetByNameAsync(name);

                if (tag == null)
                {
                    tag = new Tag
                    {
                        Id = IdentityGenerator.NewSequentialGuid(),
                        Name = name
                    };

                    await _unitOfWork.TagRepository.AddAsync(tag);
                }

                post.Tags.Add(tag);
            }

            await _unitOfWork.SaveAsync();

            return post;
        }
    }
}
