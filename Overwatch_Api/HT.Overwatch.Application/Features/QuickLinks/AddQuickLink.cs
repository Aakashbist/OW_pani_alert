using FluentValidation;
using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Contract.Responses;
using HT.Overwatch.Domain.Model;
using MediatR;

namespace HT.Overwatch.Application.Features.QuickLinks
{
    public class AddQuickLink
    {
        public class Command : IRequest<AddQuickLinkApiResponse>
        {
            public AddQuickLinkRequest Request { get; set; } = default!;
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
            }
        }

        public class Handler : IRequestHandler<Command, AddQuickLinkApiResponse>
        {
            private readonly IUnitOfWork _uow;

            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }

            public async Task<AddQuickLinkApiResponse> Handle(Command command, CancellationToken cancellationToken)
            {
                var quickLink = QuickLink.CreateQuickLink(command.Request.Id, command.Request.SiteId, command.Request.Url, command.Request.Name);

                _uow.GetRepository<QuickLink>().Add(quickLink);

                var result = await _uow.SaveChangesAsync();

                return new AddQuickLinkApiResponse() { Success = result > 0, QuickLinkId = quickLink.Id };
            }
        }
    }
}
