using System;
using dinopays.web.Controllers;

namespace dinopays.web.Models
{
    public class Pet
    {
        public Guid Id { get; set; }

        public int Health { get; set; }

        public Summary Summary { get; set; }
    }
}
