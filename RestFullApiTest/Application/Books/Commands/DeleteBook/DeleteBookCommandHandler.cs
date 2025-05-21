using MediatR;

namespace RestFullApiTest
{
    public class DeleteBookCommandHandler(IBookRepository bookRepository) : IRequestHandler<DeleteBookCommand, int>
    {
        public async Task<int> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var dto = request.DeleteBookDto;

            var result =  await bookRepository.DeleteBook(dto);

            return result;
        }
    }
}
