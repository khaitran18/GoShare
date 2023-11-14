﻿using Domain.DataModels;
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
    public class RatingRepository : BaseRepository<Rating>, IRatingRepository
    {
        private readonly GoShareContext _context;
        public RatingRepository(GoShareContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Rating?> GetRatingByUserAndTrip(Guid userId, Guid tripId)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(r => r.Rater == userId && r.TripId == tripId);
        }
    }
}
