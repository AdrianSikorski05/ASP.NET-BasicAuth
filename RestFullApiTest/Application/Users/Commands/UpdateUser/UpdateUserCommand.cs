using MediatR;

namespace RestFullApiTest
{
    public record UpdateUserCommand(UpdateUserDto UpdateUserDto) : IRequest<(User, int)>;

}
