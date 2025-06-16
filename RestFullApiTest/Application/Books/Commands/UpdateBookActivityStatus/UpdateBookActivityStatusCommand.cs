using MediatR;

namespace RestFullApiTest
{
    public record UpdateBookActivityStatusCommand(ActivityBook ActivityBook) : IRequest<bool>;

}
