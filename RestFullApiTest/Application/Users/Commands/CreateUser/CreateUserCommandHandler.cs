using MediatR;

namespace RestFullApiTest
{
    public class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, User>
    {
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CreateUserDto;
            var result = await userRepository.AddUser(dto);

            return result;
        }
    }
}
