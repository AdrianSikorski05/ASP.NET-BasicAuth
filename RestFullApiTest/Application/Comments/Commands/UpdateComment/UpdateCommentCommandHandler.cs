using MediatR;

namespace RestFullApiTest
{
    public class UpdateCommentCommandHandler(ICommentRepository commentRepository) : IRequestHandler<UpdateCommentCommand, CommentBook>
    {
        public async Task<CommentBook> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var commentBook = request.UpdateCommentBookDto;
            var result = await commentRepository.UpdateCommentAsync(commentBook);
            return result;
        }
    }
}
