using System;
using System.Collections.Generic;

namespace dinopays.web.Models
{
    public class Goal
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public IEnumerable<string> MatchingTransactions { get; set; }

        public decimal Threshold { get; set; }

        public GoalDirection GoalDirection { get; set; }

        public Frequency Frequency { get; set; }
    }
}
