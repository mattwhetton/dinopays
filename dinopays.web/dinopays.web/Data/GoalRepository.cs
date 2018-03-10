using System.Collections.Generic;
using dinopays.web.Models;

namespace dinopays.web.Data
{
    public class GoalRepository : IGoalRepository
    {
        private static readonly List<Goal> AllGoals = new List<Goal>
        {
            new Goal
            {
                Name = "Spend less than £10 per week on Coffee",
                Frequency = Frequency.Weekly,
                GoalDirection = GoalDirection.Under,
                MatchingTransactions = new[]
                {
                    "Starbucks",
                    "200degrees",
                    "Costa",
                    "CafeNero"
                },
                Threshold = 10m
            }
        };

        public IEnumerable<Goal> Goals => AllGoals.AsReadOnly();
    }
}