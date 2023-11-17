﻿using Application.Common.Exceptions;
using Application.Common.Utilities;
using Domain.DataModels;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Handlers
{
    public class SetPasscodeCommandHandler : IRequestHandler<SetPasscodeCommand, Task>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SetPasscodeCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Task> Handle(SetPasscodeCommand request, CancellationToken cancellationToken)
        {
            User? u = await _unitOfWork.UserRepository.GetUserByPhone(request.Phone);
            if (u == null) throw new NotFoundException("Phone number is not found");
            else
            {
                if (u.PasscodeResetTokenExpiryTime?.CompareTo(DateTimeUtilities.GetDateTimeVnNow()) < 0) throw new UnauthorizedAccessException("Timeout! Please verify the account again");
                else
                {
                    if (!PasswordHasher.Validate(u.PasscodeResetToken!, request.SetToken)) throw new Exception("Error in receiving data");
                    else
                    {
                        u.Passcode = PasswordHasher.Hash(request.Passcode);
                        await _unitOfWork.UserRepository.UpdateAsync(u);
                        Wallet w = new Wallet()
                        {
                            Balance = 0,
                            CreateTime = DateTimeUtilities.GetDateTimeVnNow(),
                            UpdatedTime = DateTimeUtilities.GetDateTimeVnNow(),
                            Id = Guid.NewGuid(),
                            Type = Domain.Enumerations.WalletStatus.PERSONAL,
                            UserId = u.Id
                        };
                        await _unitOfWork.WalletRepository.AddAsync(w);
                        await _unitOfWork.Save();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
