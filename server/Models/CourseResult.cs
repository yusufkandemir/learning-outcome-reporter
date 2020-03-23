using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class CourseResult
    {
        public CourseResult()
        {
            AssignmentResults = new HashSet<AssignmentResult>();
        }

        public int Id { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public string StudentId { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<AssignmentResult> AssignmentResults { get; set; }
    }
}
