using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Interfaces;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GoShareContext _context;
        private readonly IMapper _mapper;
        private IUserRepository _userRepository = null!;
        private IAppfeedbackRepository _appfeedbackRepository = null!;
        private ITripRepository _tripRepository = null!;
        private ILocationRepository _locationRepository = null!;
        private ICartypeRepository _cartypeRepository = null!;
        private ICarRepository _carRepository = null!;
        private IDriverDocumentRepository _driverDocumentRepository = null!;
        public UnitOfWork(GoShareContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context, _mapper);
        public IAppfeedbackRepository AppfeedbackRepository => _appfeedbackRepository ??= new AppfeedbackRepository(_context);
        public ITripRepository TripRepository => _tripRepository ??= new TripRepository(_context);
        public ILocationRepository LocationRepository => _locationRepository ??= new LocationRepository(_context);
        public ICartypeRepository CartypeRepository => _cartypeRepository ??= new CartypeRepository(_context);
        public ICarRepository CarRepository => _carRepository ??= new CarRepository(_context);
        public IDriverDocumentRepository DriverDocumentRepository => _driverDocumentRepository ??= new DriverDocumentRepository(_context);

        public void Dispose()
        {
            _context.DisposeAsync();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
