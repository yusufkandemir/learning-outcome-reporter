using System;
using System.Collections.Generic;

namespace server.DTOs
{
    public class ImportExcelDTO
    {
        public int CourseId { get; set; }

        public Dictionary<string, StudentResultDTO> StudentResults { get; set; }
    }

    public class StudentResultDTO
    {
        public string Name { get; set; }

        public Dictionary<int, Dictionary<int, decimal>> AssignmentTaskResults { get; set; }
    }
}