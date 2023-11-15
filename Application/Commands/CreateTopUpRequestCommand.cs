
using Domain.Enumerations;
using MediatR;

namespace Application.Commands
{
    public record CreateTopUpRequestCommand : IRequest<string>
    {
        public double Amount { get; set; }
        public TopupMethod Method { get; set; }
    }
}
