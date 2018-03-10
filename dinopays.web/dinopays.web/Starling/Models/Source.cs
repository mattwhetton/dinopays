using System.Runtime.Serialization;

namespace dinopays.web.Starling.Models
{
    public enum Source
    {
        [EnumMember(Value = "DIRECT_CREDIT")]
        DirectCredit,
        [EnumMember(Value = "DIRECT_DEBIT")]
        DirectDebit,
        [EnumMember(Value = "DIRECT_DEBIT_DISPUTE")]
        DirectDebitDispute,
        [EnumMember(Value = "INTERNAL_TRANSFER")]
        InternalTransfer,
        [EnumMember(Value = "MASTER_CARD")]
        MasterCard,
        [EnumMember(Value = "FASTER_PAYMENTS_IN")]
        FasterPaymentsIn,
        [EnumMember(Value = "FASTER_PAYMENTS_OUT")]
        FasterPaymentsOut,
        [EnumMember(Value = "FASTER_PAYMENTS_REVERSAL")]
        FasterPaymentsReversal,
        [EnumMember(Value = "STRIPE_FUNDING")]
        StripeFunding,
        [EnumMember(Value = "INTEREST_PAYMENT")]
        InterestPayment,
        [EnumMember(Value = "NOSTRO_DEPOSIT")]
        NostroDeposit,
        [EnumMember(Value = "OVERDRAFT")]
        Overdraft
    }
}