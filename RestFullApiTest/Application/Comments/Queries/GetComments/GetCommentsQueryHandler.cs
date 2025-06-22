using MediatR;

namespace RestFullApiTest
{
    public class GetCommentsQueryHandler(ICommentRepository commentRepository) : IRequestHandler<GetCommentsQuery, PagedResult<CommentBook>>
    {
        public async Task<PagedResult<CommentBook>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var filter = request.CommentFilter;
            var result = await commentRepository.GetCommentsAsync(filter);
            return result;
        }
    }
}
