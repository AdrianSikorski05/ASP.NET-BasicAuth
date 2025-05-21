using MediatR;

namespace RestFullApiTest
{
    public record UpdateBookCommand(UpdateBookDto UpdateBookDto) : IRequest<(Book, int)>;
    
}
