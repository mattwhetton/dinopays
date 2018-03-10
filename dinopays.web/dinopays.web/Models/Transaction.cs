using System;
using dinopays.web.Starling.Models;

namespace dinopays.web.Models
{
    public class Transaction
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public Direction Direction { get; set; }

        public SpendingCategory SpendingCategory { get; set; }

        public PositivityCategory PositivityCategory { get; set; }
    }
}
