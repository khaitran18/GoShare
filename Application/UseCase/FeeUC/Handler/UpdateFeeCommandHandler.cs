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
    public class UpdateFeeCommandHandler : IRequestHandler<UpdateFeeCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFeeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateFeeCommand request, CancellationToken cancellationToken)
        {
            var list = await _unitOfWork.FeeRepository.GetAllAsync();
            var f = list.FirstOrDefault(f => f.Id.CompareTo(request.Guid) == 0);
            if (f != null)
            {
                f.BaseDistance = request.base_distance;
                f.BasePrice = request.base_price;
                f.UpdateTime = DateTimeUtilities.GetDateTimeVnNow();
                await _unitOfWork.FeeRepository.UpdateAsync(f);
            }
            else throw new NotFoundException("Fee id is not found");
            await _unitOfWork.Save();
            return true;
        }
    }
}
