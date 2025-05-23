using MediatR;

namespace RestFullApiTest
{
    public record GetAllUsersQuery(UserFilter UserFilter):IRequest<PagedResult<UserDto>>;
    
}
