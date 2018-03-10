using System.Runtime.Serialization;

namespace dinopays.web.Starling.Models
{
    public enum SpendingCategory
    {
        [EnumMember(Value = "BILLS_AND_SERVICES")]
        BillsAndServices,
        [EnumMember(Value = "EATING_OUT")]
        EatingOut,
        [EnumMember(Value = "ENTERTAINMENT")]
        Entertainment,
        [EnumMember(Value = "EXPENSES")]
        Expenses,
        [EnumMember(Value = "GENERAL")]
        General,
        [EnumMember(Value = "GROCERIES")]
        Groceries,
        [EnumMember(Value = "SHOPPING")]
        Shopping,
        [EnumMember(Value = "HOLIDAYS")]
        Holidays,
        [EnumMember(Value = "PAYMENTS")]
        Payments,
        [EnumMember(Value = "TRANSPORT")]
        Transport,
        [EnumMember(Value = "LIFESTYLE")]
        Lifestyle,
        [EnumMember(Value = "NONE")]
        None
    }
}