using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Application.Features.Categories.Queries
{
    public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, IList<Category>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public GetCategoriesQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Category>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork
            .CategoryRepository
            .SearchByNameAsync(query.SearchTerm);
        }
    }
}
