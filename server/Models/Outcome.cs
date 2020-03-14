using System;
using System.Collections.Generic;

namespace server.Models
{
    public abstract class Outcome
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
