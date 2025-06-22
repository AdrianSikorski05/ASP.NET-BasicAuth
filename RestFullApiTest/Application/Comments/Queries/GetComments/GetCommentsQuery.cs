using MediatR;

namespace RestFullApiTest
{
    public record GetCommentsQuery(CommentFilter CommentFilter) : IRequest<PagedResult<CommentBook>>;
}
