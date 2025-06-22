using MediatR;

namespace RestFullApiTest
{
    public record CreateCommentCommand(CreateCommentBookDto comment) : IRequest<CommentBook>;    
}
