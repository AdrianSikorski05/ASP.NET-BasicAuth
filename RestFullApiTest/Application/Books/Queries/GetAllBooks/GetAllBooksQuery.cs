using MediatR;


namespace RestFullApiTest
{
    public record GetAllBooksQuery(BookFilter Filter):IRequest<PagedResult<BookDto>>;

}
