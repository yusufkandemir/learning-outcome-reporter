﻿using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class Department
    {
        public Department()
        {
            CourseInfos = new HashSet<CourseInfo>();
            Outcomes = new HashSet<Outcome>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CourseInfo> CourseInfos { get; set; }
        public virtual ICollection<Outcome> Outcomes { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
