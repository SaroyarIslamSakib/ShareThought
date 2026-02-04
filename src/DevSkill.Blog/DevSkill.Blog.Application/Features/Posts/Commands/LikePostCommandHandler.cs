using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Posts.Commands
{
    public class LikePostCommandHandler : ICommandHandler<LikePostCommand, int>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public LikePostCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(LikePostCommand command, CancellationToken cancellationToken)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(command.PostId);
            if (post == null)
                throw new Exception("Post not found");
            post.Likes += 1;
            await _unitOfWork.PostRepository.EditAsync(post);
            await _unitOfWork.SaveAsync();
            return post.Likes;
        }
    }
}
