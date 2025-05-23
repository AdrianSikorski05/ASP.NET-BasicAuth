using MediatR;

namespace RestFullApiTest
{
    public record DeleteBookByIdCommand(DeleteBookByIdDto DeleteBookByIdDto) : IRequest<int>;
    
}
