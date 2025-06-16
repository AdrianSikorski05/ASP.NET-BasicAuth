using MediatR;

namespace RestFullApiTest
{
    public record GetBookWithActivityStatusByUserQuery(DataStatusBookWithUserIdDto Data) : IRequest<List<Book>>;
}
