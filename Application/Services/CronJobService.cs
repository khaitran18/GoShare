using Application.Services.Interfaces;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CronJobService : ICronJobService
    {
        public CronJobService()
        {
        }

        public void Start()
        {
            RecurringJob.AddOrUpdate<BackgroundServices>("CheckDriverDebts", service => service.CheckDriverDebts(), Cron.Daily(17, 0));
            RecurringJob.AddOrUpdate<BackgroundServices>("CheckDriverRating", service => service.CheckDriverRating(), Cron.Daily(17, 5));
        }
    }
}
