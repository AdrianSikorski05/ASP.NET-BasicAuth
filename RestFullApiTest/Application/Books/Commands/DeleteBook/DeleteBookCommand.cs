using MediatR;

namespace RestFullApiTest
{
    public record DeleteBookCommand(DeleteBookDto DeleteBookDto) : IRequest<int>;

}
