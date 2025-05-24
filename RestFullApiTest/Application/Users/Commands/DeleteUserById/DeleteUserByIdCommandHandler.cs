using MediatR;

namespace RestFullApiTest
{
    public class DeleteUserByIdCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserByIdCommand, int>
    {
        public async Task<int> Handle(DeleteUserByIdCommand request, CancellationToken cancellationToken)
        {
            var userId = request.DeleteUserByIdDto;
            var result = await userRepository.DeleteUserById(userId.Id);
            return result;
        }
    }
}
