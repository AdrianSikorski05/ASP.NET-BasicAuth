using MediatR;

namespace RestFullApiTest
{
    public class GetBookWithActivityStatusByUserHandler(IBookRepository bookRepository) : IRequestHandler<GetBookWithActivityStatusByUserQuery, List<Book>>
    {
        public async Task<List<Book?>> Handle(GetBookWithActivityStatusByUserQuery request, CancellationToken cancellationToken)
        {       
            return await bookRepository.GetBookWithActivityStatusByUser(request.Data);
        }
    }
}
