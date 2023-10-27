using Application.Common.Dtos;
using Domain.DataModels;
using MediatR;

namespace Application.Commands
{
    public record AddCarCommand:IRequest<Car>
    {
        public Guid UserId { get; set; }
        public short Capacity { get; set; }
        public CarDto Car { get; set; } = null!;
    }
}
