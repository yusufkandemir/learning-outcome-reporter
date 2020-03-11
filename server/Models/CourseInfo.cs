using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class CourseInfo
    {
        public CourseInfo()
        {
            Courses = new HashSet<Course>();
            Outcomes = new HashSet<Outcome>();
        }

        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Credit { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Outcome> Outcomes { get; set; }
    }
}
