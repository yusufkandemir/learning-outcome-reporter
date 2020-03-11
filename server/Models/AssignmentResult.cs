using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class AssignmentResult
    {
        public AssignmentResult()
        {
            AssignmentTaskResults = new HashSet<AssignmentTaskResult>();
        }

        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public int CourseResultId { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual CourseResult CourseResult { get; set; }
        public virtual ICollection<AssignmentTaskResult> AssignmentTaskResults { get; set; }
    }
}
