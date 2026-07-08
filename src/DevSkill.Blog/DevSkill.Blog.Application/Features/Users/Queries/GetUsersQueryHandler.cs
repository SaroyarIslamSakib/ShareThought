using Cortex.Mediator.Queries;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Dtos;

namespace DevSkill.Blog.Application.Features.Users.Queries
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, (IList<UserListDto>, int total, int totalDisplay)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetUsersQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<UserListDto>, int total, int totalDisplay)> Handle(
            GetUsersQuery query,
            CancellationToken cancellationToken)
        {
            var procedureName = "GetUsers";

            var output = await _unitOfWork.SqlUtility
                .QueryWithStoredProcedureAsync<UserListDto>(
                    procedureName,
                    new Dictionary<string, object?>
                    {
                        { "PageIndex", query.PageIndex },
                        { "PageSize", query.PageSize },
                        { "OrderBy", query.SortOrder },

                        { "Name", string.IsNullOrWhiteSpace(query.Name) ? null : query.Name },
                        { "RegistrationFrom", query.RegistrationFrom },
                        { "RegistrationTo", query.RegistrationTo }
                    },
                    new Dictionary<string, Type>
                    {
                        { "Total", typeof(int) },
                        { "TotalDisplay", typeof(int) }
                    });

            return (
                output.result,
                (int)output.outValues["Total"],
                (int)output.outValues["TotalDisplay"]
            );
        }
    }
}
