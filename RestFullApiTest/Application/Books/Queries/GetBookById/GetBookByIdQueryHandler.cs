using MediatR;

namespace RestFullApiTest
{
    public class GetBookByIdQueryHandler(IBookRepository bookRepository) : IRequestHandler<GetBookByIdQuery, BookDto>
    {
        public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var id = request.Id;

            var result = await bookRepository.GetBookById(id);

            return new BookDto
            {
                Id = result.Id,
                Title = result.Title,
                Author = result.Author,
                Genre = result.Genre,
                Price = result.Price,
                Stock = result.Stock
            };
        }
    }
}
