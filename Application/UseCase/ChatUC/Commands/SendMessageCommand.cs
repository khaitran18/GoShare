using MediatR;

namespace Application.UseCase.ChatUC.Commands
{
    public record SendMessageCommand : IRequest<Task>
    {
        public Guid Receiver { get; set; }
        public string Content { get; set; } = null!;
    }
}