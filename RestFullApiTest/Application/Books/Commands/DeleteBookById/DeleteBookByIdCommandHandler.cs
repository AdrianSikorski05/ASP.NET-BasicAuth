using MediatR;

namespace RestFullApiTest
{
    public class DeleteBookByIdCommandHandler(IBookRepository bookRepository) : IRequestHandler<DeleteBookByIdCommand, int>
    {
        public async Task<int> Handle(DeleteBookByIdCommand request, CancellationToken cancellationToken)
        {
            var dto = request.DeleteBookByIdDto;
            var results = await bookRepository.DeleteBookById(dto.Id);

            return results;
        }
    }
}
