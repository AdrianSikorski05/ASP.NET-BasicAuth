using MediatR;

namespace RestFullApiTest
{
    public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand, int>
    {
        public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userId = request.DeleteUserDto;
            var result = await userRepository.DeleteUser(userId);
            return result;
        }
    }
}
