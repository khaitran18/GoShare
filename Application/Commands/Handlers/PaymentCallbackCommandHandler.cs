using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Utilities;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.DataModels;
using Domain.Enumerations;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class PaymentCallbackCommandHandler : IRequestHandler<PaymentCallbackCommand,bool>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserClaims _userClaims;

        public PaymentCallbackCommandHandler(IPaymentService paymentService, IUnitOfWork unitOfWork, UserClaims userClaims)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _userClaims = userClaims;
        }

        public async Task<bool> Handle(PaymentCallbackCommand request, CancellationToken cancellationToken)
        {
            var callbackResponse = _paymentService.PaymentExecute(request.collection);
            Wallettransaction transaction = await _unitOfWork.WallettransactionRepository.GetByIdAsync(new Guid(callbackResponse.vnp_TxnRef));
            if (!transaction.Status.Equals(WalletTransactionStatus.PENDING)) throw new BadRequestException("Transaction is closed");
            else
            {
                transaction.ExternalTransactionId = callbackResponse.vnp_BankTranNo;
                //VNPAY RETURN SUCCESS STATUS
                if (callbackResponse.vnp_TransactionStatus.Equals("00"))
                {
                    //Wallettransaction transaction = _unitOfWork.WallettransactionRepository.GetByIdAsync(new Guid(callbackResponse.vnp_TxnRef));
                    //transaction = _mapper.Map<Wallettransaction>(callbackResponse);
                    transaction.Status = Domain.Enumerations.WalletTransactionStatus.SUCCESSFULL;
                    transaction.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    //TOPUP TRANSACTION DESTINATION WALLET
                    Wallet wallet = _unitOfWork.WalletRepository.GetById(transaction.WalletId);
                    wallet.Balance += transaction.Amount;
                    wallet.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                    await _unitOfWork.WalletRepository.UpdateAsync(wallet);
                }
                //VNPAY RETURN FAILED STATUS
                else
                {
                    //MARK TRANSACTION AS FAILED
                    transaction.Status = Domain.Enumerations.WalletTransactionStatus.FAILED;
                    transaction.UpdatedTime = DateTimeUtilities.GetDateTimeVnNow();
                }
                await _unitOfWork.WallettransactionRepository.UpdateAsync(transaction);
                await _unitOfWork.Save();
                return true;
            }
        }
    }
}
