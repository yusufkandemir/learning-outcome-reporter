using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class AssignmentTaskResult
    {
        public int Id { get; set; }
        public int AssignmentTaskId { get; set; }
        public int AssignmentResultId { get; set; }
        public decimal Grade { get; set; }

        public virtual AssignmentResult AssignmentResult { get; set; }
        public virtual AssignmentTask AssignmentTask { get; set; }
    }
}
