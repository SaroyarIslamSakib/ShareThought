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
    public class PublishPostCommandHandler : ICommandHandler<PublishPostCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IServerTime _serverTime;
        public PublishPostCommandHandler(IApplicationUnitOfWork unitOfWork, IServerTime serverTime)
        {
            _unitOfWork = unitOfWork;
            _serverTime = serverTime;
        }
        public async Task<Guid> Handle(PublishPostCommand command, CancellationToken cancellationToken)
        {
            // Get blog by user
            var blog = (await _unitOfWork.BlogAreaRepository
                .GetByUserIdAsync(command.UserId))
                .FirstOrDefault();

            if (blog == null)
                throw new Exception("Blog not found");

            // Get draft post
            var post = await _unitOfWork.PostRepository
                .GetByIdAsync(command.PostId);

            if (post == null)
                throw new Exception("Post not found");

            // Ownership check
            if (post.BlogAreaId != blog.Id)
                throw new UnauthorizedAccessException();

            // Already published protection
            if (post.IsPublished)
                throw new Exception("Post already published");

            /* =========================
               CATEGORY PROCESS
            ==========================*/
            post.PostCategories.Clear();

            var categoryNames = command.CategoryNames
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase);

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

            /* =========================
               TAG PROCESS
            ==========================*/
            post.Tags.Clear();

            var tagNames = command.TagNames
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase);

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

            /* =========================
               FEATURE IMAGE
            ==========================*/
            post.FeatureImagePath = command.FeatureImagePath;

            /* =========================
               PUBLISH STATE CHANGE
            ==========================*/
            post.IsPublished = true;
            post.PublishedAt = _serverTime.DateTime;

            await _unitOfWork.SaveAsync();

            return post.Id;
        }
    }
}
