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
    public class SaveDraftPostCommandHandler : ICommandHandler<SaveDraftPostCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IServerTime _serverTime;
        public SaveDraftPostCommandHandler(IApplicationUnitOfWork unitOfWork, IServerTime serverTime)
        {
            _unitOfWork = unitOfWork;
            _serverTime = serverTime;
        }
        public async Task<Guid> Handle(SaveDraftPostCommand command, CancellationToken cancellationToken)
        {
            var blog = (await _unitOfWork
            .BlogAreaRepository
            .GetByUserIdAsync(command.UserId))
            .FirstOrDefault();

            if (blog == null)
                throw new Exception("Blog not found");

            Post post;

            if (command.Id.HasValue && command.Id.Value != Guid.Empty)
            {
                post = await _unitOfWork.PostRepository
                    .GetByIdAsync(command.Id.Value);

                if (post == null || post.IsPublished)
                    throw new Exception("Invalid draft");
            }
            else
            {
                // 🟢 CREATE NEW DRAFT
                post = new Post
                {
                    Id = IdentityGenerator.NewSequentialGuid(),
                    BlogAreaId = blog.Id,
                    Title = command.Title,
                    Content = command.Content,
                    CreatedAt = _serverTime.DateTime,
                    IsPublished = false
                };

                await _unitOfWork.PostRepository.AddAsync(post);
                await _unitOfWork.SaveAsync();

                return post.Id;
            }

            post.Title = command.Title;
            post.Content = command.Content;

            await _unitOfWork.SaveAsync();
            return post.Id;
        }
    }
}
