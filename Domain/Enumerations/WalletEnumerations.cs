using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enumerations
{
    public enum WalletStatus : short
    {
        SYSTEM = 0,
        PERSONAL = 1
    }

    public enum PaymentMethod : short
    {
        WALLET = 0,
        VNPAY = 1,
        CASH = 2
    }

    public enum WalletTransactionStatus : short
    {
        PENDING = 0,
        SUCCESSFULL = 1,
        FAILED = -1
    }

    public enum WalletTransactionType : short
    {
        TOPUP = 0,
        DRIVER_WAGE = 1,
        SYSTEM_COMMISSION = 2,
        PASSENGER_PAYMENT = 3,
        DRIVER_DEPOSIT = 4,
        PASSENGER_REFUND = 5,
    }
    public enum TopupMethod : short
    {
        VnPay = 0
    }
}
