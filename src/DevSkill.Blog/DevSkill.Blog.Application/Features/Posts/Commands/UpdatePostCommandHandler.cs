using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class UpdatePostCommandHandler : ICommandHandler<UpdatePostCommand, Post>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public UpdatePostCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Post> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(command.PostId);
            if (post == null)
            {
                throw new Exception("Post not found");
            }
            post.Title = command.Title;
            post.Content = command.Content;
            post.FeatureImagePath = command.FeatureImagePath;
            await _unitOfWork.PostRepository.EditAsync(post);
            await _unitOfWork.SaveAsync();
            return post;
        }
    }
}
