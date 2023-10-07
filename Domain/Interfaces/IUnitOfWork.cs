namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IAppfeedbackRepository AppfeedbackRepository { get; }
        Task Save();
    }
}
