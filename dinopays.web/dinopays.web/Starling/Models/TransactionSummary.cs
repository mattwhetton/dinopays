using System;

namespace dinopays.web.Starling.Models
{
    public class TransactionSummary
    {
        public Guid Id { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public Direction Direction { get; set; }

        public DateTimeOffset Created { get; set; }

        public string Narrative { get; set; }

        public Source Source { get; set; }

        public decimal Balance { get; set; }
    }
}