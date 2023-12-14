using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.UseCase.FeeUC.Command;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.FeeUC.Handler
{
    public class UpdateFeePolicyCommandHandler : IRequestHandler<UpdateFeePolicyCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFeePolicyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateFeePolicyCommand request, CancellationToken cancellationToken)
        {
            var fees = await _unitOfWork.FeeRepository.GetAllWithPoliciesAsync();
            var fee = fees.FirstOrDefault(f => f.Feepolicies.Any(p => p.Id.CompareTo(request.Guid) == 0));
            if (fee == null) throw new NotFoundException("Fee policy is not found");
            else
            {
                var policy = fee!.Feepolicies.FirstOrDefault(po => po.Id.CompareTo(request.Guid) == 0);
                if (policy==null) throw new NotFoundException("Fee policy is not found");
                policy.UpdateTime = DateTimeUtilities.GetDateTimeVnNow();
                //policy.MaxDistance = request.max_distance;
                //policy.MinDistance = request.min_distance;
                policy.PricePerKm = request.price_per_km;
            }
            await _unitOfWork.Save();
            return true;
        }
    }
}
