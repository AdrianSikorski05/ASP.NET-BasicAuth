using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestFullApiTest
{
    public class GetAllBooksQueryHandler(IBookRepository bookRepository) : IRequestHandler<GetAllBooksQuery, PagedResult<BookDto>>
    {

        public async Task<PagedResult<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var filter = request.Filter;

            var result = await bookRepository.GetAllAsync(filter);

            return new PagedResult<BookDto>
            {
                Items = result.Items.Select(Book.MapToDto),
                TotalItems = result.TotalItems,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}
