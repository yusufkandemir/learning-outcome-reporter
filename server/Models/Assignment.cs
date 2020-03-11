using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class Assignment
    {
        public Assignment()
        {
            AssignmentResults = new HashSet<AssignmentResult>();
            AssignmentTasks = new HashSet<AssignmentTask>();
        }

        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Type { get; set; }
        public decimal Weight { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<AssignmentResult> AssignmentResults { get; set; }
        public virtual ICollection<AssignmentTask> AssignmentTasks { get; set; }
    }
}
