using System;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.Models;

namespace dinopays.web.ApplicationServices
{
    public interface ISummaryBuilder
    {
        Task<Summary> Summarise(User user, DateTimeOffset from, DateTimeOffset to, CancellationToken cancel);
    }
}
