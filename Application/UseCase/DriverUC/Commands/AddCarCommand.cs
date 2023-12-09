using Application.Common.Dtos;
using Domain.DataModels;
using MediatR;

namespace Application.UseCase.DriverUC.Commands
{
    public record AddCarCommand : IRequest<Guid>
    {
        public Guid UserId { get; set; }
        public short Capacity { get; set; }
        public CarDto Car { get; set; } = null!;
    }
}
