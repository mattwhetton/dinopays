using System;
using System.Collections.Generic;
using dinopays.web.Starling.Models;

namespace dinopays.web.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string AccessToken { get; set; }

        public IEnumerable<Goal> Goals { get; set; }

        public Dictionary<string, string> Categories { get; set; }
    }
}
