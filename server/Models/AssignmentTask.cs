using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class AssignmentTask
    {
        public AssignmentTask()
        {
            AssignmentTaskOutcomes = new HashSet<AssignmentTaskOutcome>();
            AssignmentTaskResults = new HashSet<AssignmentTaskResult>();
        }

        public int Id { get; set; }
        public int Number { get; set; }
        public decimal Weight { get; set; }

        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
        public virtual ICollection<AssignmentTaskOutcome> AssignmentTaskOutcomes { get; set; }
        public virtual ICollection<AssignmentTaskResult> AssignmentTaskResults { get; set; }
    }
}
