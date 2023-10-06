using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Interfaces;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly postgresContext _context;
        private readonly IMapper _mapper;
        private IUserRepository _userRepository;

        public UnitOfWork(postgresContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context,_mapper);
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
