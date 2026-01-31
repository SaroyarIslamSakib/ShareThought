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
    public class UpdatePostCommandHandler
        : ICommandHandler<UpdatePostCommand, Post>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public UpdatePostCommandHandler(
            IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> Handle(
            UpdatePostCommand command,
            CancellationToken cancellationToken)
        {
            // 🔥 MUST eager load categories
            var post = await _unitOfWork.PostRepository
                .GetPostWithCategoriesAsync(command.PostId);

            if (post == null)
                throw new Exception("Post not found.");

            // 🔹 Basic fields
            post.Title = command.Title;
            post.Content = command.Content;
            post.FeatureImagePath = command.FeatureImagePath;

            // 🔹 CATEGORY UPDATE
            post.PostCategories.Clear();

            var categoryNames = command.CategoryNames?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

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

                    await _unitOfWork.CategoryRepository
                        .AddAsync(category);
                }

                post.PostCategories.Add(category);
            }

            // ❌ DO NOT call EditAsync(post)
            await _unitOfWork.SaveAsync();

            return post;
        }
    }
}
