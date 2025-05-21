using MediatR;

namespace RestFullApiTest
{
    public record CreateBookCommand(CreateBookDto Dto): IRequest<Book>;
           
}
