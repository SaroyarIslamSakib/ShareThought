using Cortex.Mediator.Commands;
using DevSkill.Blog.Domain;
using DevSkill.Blog.Domain.Entities;
using DevSkill.Blog.Domain.Utilities;

namespace DevSkill.Blog.Application.Features.Reports.Commands
{
    public class AddReportCommandHandler : ICommandHandler<AddReportCommand, Guid>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        public AddReportCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(AddReportCommand command, CancellationToken cancellationToken)
        {
            var report = new Report
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                PostId = command.PostId,
                UserId = command.UserId,
                Reason = command.Reason,
                CustomReason = command.CustomReason,
                CreatedAt = command.CreatedAt
            };
            await _unitOfWork.ReportRepository.AddAsync(report);
            await _unitOfWork.SaveAsync();
            return report.Id;
        }
    }
}
