using Domain.Enumerations;
using MediatR;

namespace Application.UseCase.PaymentUC.Commands
{
    public record CreateTopUpRequestCommand : IRequest<string>
    {
        public double Amount { get; set; }
        public TopupMethod Method { get; set; }
    }
}
