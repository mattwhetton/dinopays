namespace dinopays.web.Models
{
    public class Summary
    {
        public decimal TotalIncoming { get; set; }

        public decimal TotalOutgoing { get; set; }

        public decimal PositiveOutgoing { get; set; }

        public decimal NegativeOutgoing { get; set; }
    }
}