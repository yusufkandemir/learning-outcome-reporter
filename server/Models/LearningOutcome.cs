using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class LearningOutcome : Outcome
    {
        public int CourseInfoId { get; set; }

        public virtual CourseInfo CourseInfo { get; set; }
    }
}
