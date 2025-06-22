using MediatR;

namespace RestFullApiTest
{
    public record UpdateCommentCommand(UpdateCommentBookDto UpdateCommentBookDto) : IRequest<CommentBook>;
}
