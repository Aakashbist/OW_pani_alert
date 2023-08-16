using FluentValidation;
using HT.Overwatch.Application.Common.Exceptions;
using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Contract.Responses;
using HT.Overwatch.Domain.Model;
using MediatR;

namespace HT.Overwatch.Application.Features.QuickLinks
{
    public class UpdateQuickLink
    {
        public class Command : IRequest<ApiResponse>
        {
            public UpdateQuickLinkRequest Request { get; set; } = default!;
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(v => v.Request.SiteId)
                    .Cascade(CascadeMode.Stop)
                    .NotNull();

                RuleFor(v => v.Request.Name)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .NotNull();

                RuleFor(v => v.Request.Url)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .NotNull();

                RuleFor(v => v.Request.Id)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .NotNull()
                    .GreaterThan(0);
            }
        }

        public class Handler : IRequestHandler<Command, ApiResponse>
        {
            private readonly IUnitOfWork _uow;
            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public async Task<ApiResponse> Handle(Command command, CancellationToken cancellationToken)
            {
                var quickLink = QuickLink.CreateQuickLink(command.Request.Id, command.Request.SiteId, command.Request.Url, command.Request.Name);

                _uow.GetRepository<QuickLink>().Update(quickLink);

                var result = await _uow.SaveChangesAsync();

                return new ApiResponse() { Success = result > 0 };
            }
        }
    }
}
