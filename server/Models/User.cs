using System;
using System.Collections.Generic;

namespace server.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public bool IsAdmin => this.DepartmentId == null;
    }
}
