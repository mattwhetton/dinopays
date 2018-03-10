using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.Models;

namespace dinopays.web.ApplicationServices
{
    public interface ISummaryBuilder
    {
        Task<Summary> Summarise(DateTimeOffset from, DateTimeOffset to, CancellationToken cancel);
    }
}
