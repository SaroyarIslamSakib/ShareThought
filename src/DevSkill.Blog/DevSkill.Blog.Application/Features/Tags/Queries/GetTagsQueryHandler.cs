using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Tags.Queries
{
    public class GetTagsQueryHandler : IQueryHandler<GetTagsQuery, IList<Tag>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetTagsQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IList<Tag>> Handle(GetTagsQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.TagRepository.SearchByNameAsync(query.SearchTerm);
        }
    }
}
