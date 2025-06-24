using MediatR;

namespace RestFullApiTest
{
    public class DeleteCommentByIdCommandHandler(ICommentRepository commentRepository) : IRequestHandler<DeleteCommentByIdCommand, bool>
    {
        public async Task<bool> Handle(DeleteCommentByIdCommand request, CancellationToken cancellationToken)
        {           
            var id = request.Id;
            return  await commentRepository.DeleteCommentAsync(id);
        }
    }
}
