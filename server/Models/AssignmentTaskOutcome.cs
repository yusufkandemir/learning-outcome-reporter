using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class AssignmentTaskOutcome
    {
        public int AssignmentTaskId { get; set; }
        public virtual AssignmentTask AssignmentTask { get; set; }

        public int OutcomeId { get; set; }
        public virtual Outcome Outcome { get; set; }
    }
}
