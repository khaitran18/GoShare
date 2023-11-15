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
        //CANCEL_FEE = 4,
        //BOOKING_PAID = 5,
        //BOOKING_REFUND = 9,
        //CANCEL_REFUND = 6,
        //TRIP_PICK = 7,
        //TRIP_PICK_REFUND = 8
    }
    public enum TopupMethod : short
    {
        VnPay = 0
    }
}
