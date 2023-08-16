using FluentValidation;
using HT.Overwatch.Application.Common.Exceptions;
using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Contract.Responses;
using HT.Overwatch.Domain.Model;
using MediatR;

namespace HT.Overwatch.Application.Features.QuickLinks
{
    public class DeleteQuickLink
    {
        public class Command : IRequest<ApiResponse>
        {
            public int Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(v => v.Id)
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
                var repository = _uow.GetRepository<QuickLink>();
                var quickLink = repository.GetById(command.Id);
               
                if (quickLink is null) throw new NotFoundException("Delete Quick Link", command.Id);
                //$"Entity \"{name}\" ({key}) was not found."

                repository.Delete(quickLink);

                var result = await _uow.SaveChangesAsync();

                return new ApiResponse() { Success = result > 0 };
            }
        }
    }
}
