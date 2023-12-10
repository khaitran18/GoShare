using Domain.Enumerations;
using MediatR;

namespace Application.UseCase.TripUC.Commands
{
    public record CreateTripV2Command : IRequest<string>
    {
        public decimal StartLatitude { get; set; }
        public decimal StartLongitude { get; set; }
        public string? StartAddress { get; set; }
        public decimal EndLatitude { get; set; }
        public decimal EndLongitude { get; set; }
        public string? EndAddress { get; set; }
        public Guid CartypeId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Phone { get; set; }
        public string Name { get; set; } = null!;
        public string? Note { get; set; }
    }
}
