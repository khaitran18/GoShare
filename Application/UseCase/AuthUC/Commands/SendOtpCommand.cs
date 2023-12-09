using MediatR;

namespace Application.UseCase.AuthUC.Commands
{
    public record SendOtpCommand : IRequest<bool>
    {
        public string? phone;
    }
}
