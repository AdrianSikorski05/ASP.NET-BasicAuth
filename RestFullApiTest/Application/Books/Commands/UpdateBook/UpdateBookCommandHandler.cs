using MediatR;

namespace RestFullApiTest
{
    public class UpdateBookCommandHandler(IBookRepository bookRepository) : IRequestHandler<UpdateBookCommand, (Book, int)>
    {

        public async Task<(Book,int)> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var updateBook = request.UpdateBookDto;

            var result = await bookRepository.UpdateBook(updateBook);
            return result;
        }
    }
}
