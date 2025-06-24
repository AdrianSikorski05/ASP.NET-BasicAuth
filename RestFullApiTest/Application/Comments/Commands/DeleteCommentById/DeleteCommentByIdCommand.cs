using MediatR;

namespace RestFullApiTest
{
    public record DeleteCommentByIdCommand(int Id) : IRequest<bool>;
}
