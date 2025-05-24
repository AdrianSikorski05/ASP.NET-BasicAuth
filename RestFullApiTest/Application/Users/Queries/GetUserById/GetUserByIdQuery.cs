using MediatR;

namespace RestFullApiTest
{
    public record GetUserByIdQuery(int Id) : IRequest<UserDto>;
}
