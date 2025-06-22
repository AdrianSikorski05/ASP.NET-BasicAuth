using MediatR;

namespace RestFullApiTest
{
    public class CreateCommentCommandHandler(ICommentRepository commentRepository) : IRequestHandler<CreateCommentCommand, CommentBook>
    {
        public async Task<CommentBook> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = request.comment;

            var result = await commentRepository.AddCommentAsync(comment);
            return result;
        }
    }
}
