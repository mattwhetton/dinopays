using System.Runtime.Serialization;

namespace dinopays.web.Starling.Models
{
    public enum Direction
    {
        [EnumMember(Value = "NONE")]
        None,
        [EnumMember(Value = "INBOUND")]
        Inbound,
        [EnumMember(Value = "OUTBOUND")]
        Outbound
    }
}