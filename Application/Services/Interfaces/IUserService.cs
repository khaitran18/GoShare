using Application.Common.Dtos;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<DependentTripInfo>?> GetCurrentDenpendentTrips(IUnitOfWork unitOfWork, Guid UserId);
        Task<bool> CheckDependentStatus(IUnitOfWork unitOfWork, Guid UserId, Guid GuadianId);
    }
}
