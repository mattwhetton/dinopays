namespace dinopays.web.Models
{
    public class GoalStatus
    {
        public Goal Goal { get; set; }

        public decimal CurrentSpend { get; set; }

        public bool OnTarget { get; set; }
    }
}