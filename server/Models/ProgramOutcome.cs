using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class ProgramOutcome : Outcome
    {
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
