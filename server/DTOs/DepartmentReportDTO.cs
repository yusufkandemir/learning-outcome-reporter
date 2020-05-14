using System;
using System.Collections.Generic;

using server.Models;

namespace server.DTOs
{
    public class DepartmentReportInputDTO
    {
        public int DepartmentId { get; set; }

        public Semester Semester { get; set; }

        public int Year { get; set; }
    }

    public class DepartmentReportOutputDTO
    {
        public List<ResultPair> Results { get; set; } // [{x: '1', y: 44}]
    }

    public class ResultPair
    {
        public string x { get; set; }

        public decimal y { get; set; }
    }
}