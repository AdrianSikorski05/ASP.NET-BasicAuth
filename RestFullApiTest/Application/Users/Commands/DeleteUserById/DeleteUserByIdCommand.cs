using MediatR;

namespace RestFullApiTest
{
    public record DeleteUserByIdCommand(DeleteUserByIdDto DeleteUserByIdDto) : IRequest<int>;
}
