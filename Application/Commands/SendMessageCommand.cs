using MediatR;

namespace Application.Commands
{
    public record SendMessageCommand : IRequest<Task>
    {
        public Guid Receiver { get; set; }
        public string Content { get; set; } = null!;
    }
}