using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services;
using Application.Services.Interfaces;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using Hangfire;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class CreateTopupRequestCommandHandler : IRequestHandler<CreateTopUpRequestCommand, string>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;

        public CreateTopupRequestCommandHandler(IPaymentService paymentService, IUnitOfWork unitOfWork, UserClaims userClaims)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
        }

        public async Task<string> Handle(CreateTopUpRequestCommand request, CancellationToken cancellationToken)
        {
            string response = "";

            var wallet = await _unitOfWork.WalletRepository.GetByUserIdAsync((Guid)_userClaims.id!);
            if (wallet is null) throw new NotFoundException("User's wallet not found");

            var transaction = new Wallettransaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet!.Id,
                Amount = request.Amount,
                PaymentMethod = PaymentMethod.VNPAY,
                Status = WalletTransactionStatus.PENDING,
                Type = WalletTransactionType.TOPUP,
                CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                UpdatedTime = DateTimeUtilities.GetDateTimeVnNow()
            };
            await _unitOfWork.WallettransactionRepository.AddAsync(transaction);

            if (request.Method.Equals(TopupMethod.VnPay))
            {
                response = await _paymentService.CreateVnpayTopupRequest(_userClaims, request.Amount,transaction.Id);
                if (response.Equals("")) throw new Exception("Error in creating Vnpay request");
            }

            await _unitOfWork.Save();
            BackgroundJob.Schedule<BackgroundServices>(s => s.CheckTransactionStatus(transaction.Id), TimeSpan.FromMinutes(15));
            return response;
        }
    }
}
