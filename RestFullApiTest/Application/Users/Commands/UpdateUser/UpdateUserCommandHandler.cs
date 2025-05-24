using MediatR;

namespace RestFullApiTest
{
    public class UpdateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserCommand, (User,int)>
    {
        public async Task<(User, int)> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var updateUserDto = request.UpdateUserDto;

            var result = await userRepository.UpdateUser(updateUserDto);

            return result;
        }
    }
}
