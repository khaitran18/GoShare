namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IAppfeedbackRepository AppfeedbackRepository { get; }
        ITripRepository TripRepository { get; }
        ILocationRepository LocationRepository { get; }
        Task Save();
    }
}
