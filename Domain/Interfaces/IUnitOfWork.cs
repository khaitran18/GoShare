namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IAppfeedbackRepository AppfeedbackRepository { get; }
        ITripRepository TripRepository { get; }
        ILocationRepository LocationRepository { get; }
        ICartypeRepository CartypeRepository { get; }
        ICarRepository CarRepository { get; }
        IChatRepository ChatRepository { get; }
        IDriverDocumentRepository DriverDocumentRepository { get; }
        IWalletRepository WalletRepository { get; }
        IWallettransactionRepository WallettransactionRepository { get; }
        ISettingRepository SettingRepository { get; }
        IRatingRepository RatingRepository { get; }
        IReportRepository ReportRepository { get; }

        Task Save();
    }
}
