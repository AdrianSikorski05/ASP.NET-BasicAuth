using MediatR;

namespace RestFullApiTest
{
    public class GetUserByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var id = request.Id;

            var result = await  userRepository.GetUserById(id);

            return new UserDto
            {
                Id = result.Id,
                Username = result.Username
            };
        }
    }
}
