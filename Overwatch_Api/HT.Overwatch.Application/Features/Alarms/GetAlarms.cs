using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.Alarms;
using MediatR;

namespace HT.Overwatch.Application.Features.Alarms
{
    public class GetAlarms
    {
        public class Query : IRequest<IEnumerable<AlarmResponse>>
        {
        }

        public class Handler : IRequestHandler<Query, IEnumerable<AlarmResponse>>
        {
            private readonly IUnitOfWork _uow;
            private readonly IDateTime _dt;
            public Handler(IUnitOfWork uow, IDateTime dt)
            {
                _uow = uow;
                _dt = dt;   
            }
            public async Task<IEnumerable<AlarmResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                await Task.Delay(0);

                return new List<AlarmResponse>();
            }
        }
    }
}
