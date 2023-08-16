using HT.Overwatch.Application.Common;
using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.Common;
using HT.Overwatch.Application.ResponseModels.TimeseriesValue;
using HT.Overwatch.Domain.Model.TimeSeriesStorage;
using MediatR;

namespace HT.Overwatch.Application.Features.TimeseriesValues
{
    public class GetTimeseriesValues
    {
        public class Query : IRequest<PaginationResult<TimeseriesValueResponse>>
        {
            public int currentPage { get; set; }
            public int PageSize { get; set; }
        }

        public class Handler : IRequestHandler<Query, PaginationResult<TimeseriesValueResponse>>
        {
            private readonly IUnitOfWork _uow;

            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public Task<PaginationResult<TimeseriesValueResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var filteredData = _uow.GetRepository<TimeSeriesValue>()
                             .Get(d => d.Time, Common.Enums.SortOrder.Descending);

                var totalItems = filteredData.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);
                if (request.currentPage > totalPages)
                {
                    request.currentPage = totalPages;
                }

                var previousPage = request.currentPage <= 1 ? 1 : request.currentPage - 1;
                if (previousPage == 0)
                {
                    previousPage = 1;
                }

                var nextPage = request.currentPage >= totalPages ? totalPages : request.currentPage + 1;

                var itemsForCurrentPage = filteredData.Skip((request.currentPage - 1) * request.PageSize)
                                                       .Take(request.PageSize)
                                                       .AsQueryable();

                return Task.FromResult(new PaginationResult<TimeseriesValueResponse>
                {
                    TotalPages = totalPages,
                    PreviousPage = previousPage,
                    NextPage = nextPage,
                    TotalItems = totalItems,
                    Items = itemsForCurrentPage.Select(x => new TimeseriesValueResponse
                    {
                        Time = x.Time,
                        TimeSeriesId = x.TimeSeriesId,
                        Quality = x.Quality,
                        Value = x.Value
                    }).ToList()
                });
            }
        }

    }
}
