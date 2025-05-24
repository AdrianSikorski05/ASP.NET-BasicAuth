using MediatR;

namespace RestFullApiTest
{
    public record DeleteUserCommand(DeleteUserDto DeleteUserDto) : IRequest<int>;
}
