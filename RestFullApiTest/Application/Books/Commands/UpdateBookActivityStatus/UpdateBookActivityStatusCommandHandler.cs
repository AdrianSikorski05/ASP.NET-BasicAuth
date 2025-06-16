using MediatR;

namespace RestFullApiTest
{
    public class UpdateBookActivityStatusCommandHandler(IBookRepository bookRepository): IRequestHandler<UpdateBookActivityStatusCommand, bool>
    {
        public async Task<bool> Handle(UpdateBookActivityStatusCommand request, CancellationToken cancellationToken)
        {
            var updateBookActivity = request.ActivityBook;
            var result = await bookRepository.UpdateBookActivityStatus(updateBookActivity);
            return result;
        }
    }
}
