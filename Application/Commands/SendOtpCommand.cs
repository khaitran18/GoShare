using MediatR;

namespace Application.Commands
{
    public record SendOtpCommand : IRequest<bool>
    {
        public string? phone;
    }
}
