using AutoMapper;
using Domain.DataModels;
using Domain.Interfaces;
using Infrastructure.Data;


namespace Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly GoShareContext _context;
        private readonly IMapper _mapper;

        public UserRepository(GoShareContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
