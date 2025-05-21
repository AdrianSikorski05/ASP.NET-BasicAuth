using MediatR;

namespace RestFullApiTest
{
    public class CreateBookCommandHandler(IBookRepository bookRepository) : IRequestHandler<CreateBookCommand, Book>
    {
        public async Task<Book> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
          
            var savedBook = await bookRepository.AddBook(dto);
            return savedBook;
        }
    }
}
