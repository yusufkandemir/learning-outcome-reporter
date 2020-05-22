using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using server.Models;
using server.DTOs;

namespace server.Controllers
{
    [Route("api/Reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly OutcomeReportingContext _context;

        public ReportController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Reports/Department
        [Route("Department")]
        [HttpGet]
        public async Task<ActionResult<DepartmentReportOutputDTO>> DepartmentReport([FromQuery] DepartmentReportInputDTO input)
        {
            var courses = await _context.Courses
                .Include(x => x.CourseInfo)
                .Include(x => x.CourseResults)
                .Where(x => x.Semester == input.Semester && x.Year == input.Year && x.CourseInfo.DepartmentId == input.DepartmentId)
                .ToListAsync();

            var courseIds = courses.Select(x => x.Id).ToArray();
            var courseResults = await _context.CourseResults
                .Include(x => x.AssignmentResults)
                    .ThenInclude(x => x.Assignment)
                .Include(x => x.AssignmentResults)
                    .ThenInclude(x => x.AssignmentTaskResults)
                    .ThenInclude(x => x.AssignmentTask)
                    .ThenInclude(x => x.Outcomes)
                    .ThenInclude(x => x.Outcome)
                .Where(x => courseIds.Contains(x.CourseId))
                .ToListAsync();

            var grades = new Dictionary<byte, Dictionary<string, List<(decimal taskWeight, decimal assignmentWeight, decimal courseCredit, decimal grade)>>>();
            var credits = new Dictionary<byte, Dictionary<int, int>>();

            foreach (var courseResult in courseResults)
            {
                int courseId = courseResult.Course.CourseInfo.Id;
                int courseCredit = courseResult.Course.CourseInfo.Credit;
                string studentId = courseResult.StudentId;

                foreach (var assignmentResult in courseResult.AssignmentResults)
                {
                    decimal assignmentWeight = assignmentResult.Assignment.Weight;

                    foreach (var assignmentTaskResult in assignmentResult.AssignmentTaskResults)
                    {
                        decimal assignmentTaskWeight = assignmentTaskResult.AssignmentTask.Weight;
                        decimal poGrade = assignmentTaskResult.Grade * courseCredit;

                        foreach (var assignmentTaskOutcome in assignmentTaskResult.AssignmentTask.Outcomes)
                        {
                            byte poCode = assignmentTaskOutcome.Outcome.Code;

                            if (!grades.ContainsKey(poCode))
                            {
                                grades[poCode] = new Dictionary<string, List<(decimal taskWeight, decimal assignmentWeight, decimal courseCredit, decimal grade)>>();
                            }

                            if (!grades[poCode].ContainsKey(studentId))
                            {
                                grades[poCode][studentId] = new List<(decimal taskWeight, decimal assignmentWeight, decimal courseCredit, decimal grade)>();
                            }

                            grades[poCode][studentId].Add((assignmentTaskWeight, assignmentWeight, courseCredit, poGrade));

                            if (!credits.ContainsKey(poCode))
                            {
                                credits[poCode] = new Dictionary<int, int>();
                            }

                            credits[poCode][courseId] = courseCredit;
                        }
                    }
                }
            }

            var results = new List<ResultPair>();

            foreach (var (poCode, studentGrades) in grades.OrderBy(x => x.Key))
            {
                var totalCredits = credits[poCode].Sum(x => x.Value);

                decimal value = studentGrades.Average(studentGrade =>
                {
                    var totalWeight = studentGrade.Value.Sum(x => x.assignmentWeight * x.taskWeight * x.courseCredit);

                    return studentGrade.Value.Sum(x => x.grade * x.taskWeight * x.assignmentWeight * (x.courseCredit / totalCredits) / totalWeight);
                });

                results.Add(new ResultPair { x = poCode.ToString(), y = Math.Round(value, 2) });
            }

            var output = new DepartmentReportOutputDTO
            {
                Results = results
            };

            return Ok(output);
        }
    }
}
