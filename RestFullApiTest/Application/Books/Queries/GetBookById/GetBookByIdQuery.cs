using MediatR;

namespace RestFullApiTest
{
    public record GetBookByIdQuery(int Id) : IRequest<BookDto>;
}
