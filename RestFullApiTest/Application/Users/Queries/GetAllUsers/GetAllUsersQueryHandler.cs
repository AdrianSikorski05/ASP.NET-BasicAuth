using MediatR;

namespace RestFullApiTest
{
    public class GetAllUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<GetAllUsersQuery, PagedResult<UserDto>>
    {

        public async Task<PagedResult<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var userFilter =  request.UserFilter;

            var result = await userRepository.GetAllUsers(userFilter);

            return new PagedResult<UserDto>
            {
                Items = result.Items.Select(User.MapToDto),
                TotalItems = result.TotalItems,
                Page = result.Page,
                PageSize = result.PageSize
            };
        }
    }
}
