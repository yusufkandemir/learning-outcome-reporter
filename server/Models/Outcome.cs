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

        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int? CourseInfoId { get; set; }
        public virtual CourseInfo CourseInfo { get; set; }

        public virtual ICollection<AssignmentTaskOutcome> AssignmentTaskOutcomes { get; set; }
    }
}
