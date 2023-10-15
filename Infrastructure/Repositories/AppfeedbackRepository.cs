using Domain.DataModels;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AppfeedbackRepository : BaseRepository<Appfeedback>, IAppfeedbackRepository
    {
        private readonly GoShareContext _context;
        public AppfeedbackRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<Appfeedback>, int)> GetAppfeedbacks(string? sortBy, int page, int pageSize)
        {
            IQueryable<Appfeedback> query = _context.Appfeedbacks.Include(a => a.User).AsQueryable();

            // Sort by
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    //case "title":
                    //    query = query.OrderBy(a => a.Title);
                    //    break;
                    //case "title_desc":
                    //    query = query.OrderByDescending(a => a.Title);
                    //    break;
                    //case "content":
                    //    query = query.OrderBy(a => a.Content.Length);
                    //    break;
                    //case "content_desc":
                    //    query = query.OrderByDescending(a => a.Content.Length);
                    //    break;
                    case "time":
                        query = query.OrderBy(a => a.CreateTime);
                        break;
                    case "time_desc":
                        query = query.OrderByDescending(a => a.CreateTime);
                        break;
                    //case "username":
                    //    query = query.OrderBy(a => a.User.Name);
                    //    break;
                    //case "username_desc":
                    //    query = query.OrderByDescending(a => a.User.Name);
                    //    break;
                }
            }

            var totalCount = await query.CountAsync();

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var appFeedbacks = await query.ToListAsync();

            return (appFeedbacks, totalCount);
        }
    }
}
