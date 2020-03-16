using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class Course
    {
        public Course()
        {
            Assignments = new HashSet<Assignment>();
            CourseResults = new HashSet<CourseResult>();
        }

        public int Id { get; set; }
        public int CourseInfoId { get; set; }
        public Semester Semester { get; set; }
        public int Year { get; set; }

        public virtual CourseInfo CourseInfo { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<CourseResult> CourseResults { get; set; }
    }
}
