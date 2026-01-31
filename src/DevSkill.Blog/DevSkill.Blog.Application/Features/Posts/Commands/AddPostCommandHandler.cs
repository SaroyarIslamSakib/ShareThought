using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class AddPostCommandHandler : ICommandHandler<AddPostCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public AddPostCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(AddPostCommand command, CancellationToken cancellationToken)
        {
            var blog = (await _unitOfWork.BlogAreaRepository
                .GetByUserIdAsync(command.UserId))
                .FirstOrDefault();

            if (blog == null)
                throw new Exception("Blog not found.");

            var categoryNames = command.CategoryNames?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            var categories = new List<Category>();

            foreach (var name in categoryNames)
            {
                var category = await _unitOfWork.CategoryRepository.GetByNameAsync(name);

                if (category == null)
                {
                    category = new Category
                    {
                        Id = IdentityGenerator.NewSequentialGuid(),
                        Name = name
                    };

                    await _unitOfWork.CategoryRepository.AddAsync(category);
                }

                categories.Add(category);
            }
            var post = new Post
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                Title = command.Title,
                Content = command.Content,
                CreatedAt = command.CreatedAt,
                FeatureImagePath = command.FeatureImagePath,
                BlogAreaId = blog.Id,

            };
            // MANY TO MANY LINK
            foreach (var category in categories)
            {
                post.PostCategories.Add(category);
            }

            await _unitOfWork.PostRepository.AddAsync(post);
            await _unitOfWork.SaveAsync();

            return post.Id;
        }

    }
}
