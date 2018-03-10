using System.Collections.Generic;
using dinopays.web.Models;

namespace dinopays.web.Data
{
    public interface IGoalRepository
    {
        IEnumerable<Goal> Goals { get; }
    }
}
