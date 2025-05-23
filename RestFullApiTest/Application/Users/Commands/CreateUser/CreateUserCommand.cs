using MediatR;

namespace RestFullApiTest
{
    public record CreateUserCommand(CreateUserDto CreateUserDto) : IRequest<User>{ }
}
