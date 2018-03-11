using System;
using System.Collections.Generic;

namespace dinopays.web.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string AccessToken { get; set; }

        public IEnumerable<Goal> Goals { get; set; }
    }
}
