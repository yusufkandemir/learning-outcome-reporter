using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using server.Models;
using server.DTOs;

namespace server.Controllers
{
    [ApiController]
    public class ImportExcelController : ControllerBase
    {

        private readonly OutcomeReportingContext _context;

        public ImportExcelController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // POST: api/ImportExcel
        [Route("api/ImportExcel")]
        [HttpPost]
        public async Task<ActionResult<ImportExcelDTO>> Import(ImportExcelDTO payload)
        {
            foreach (var (studentId, studentResult) in payload.StudentResults)
            {
                var student = new Student
                {
                    Id = studentId,
                    FullName = studentResult.Name
                };

                var courseResult = new CourseResult
                {
                    CourseId = payload.CourseId,
                    StudentId = studentId,
                };

                foreach (var (assignmentId, assignmentTaskResults) in studentResult.AssignmentTaskResults)
                {
                    var assignmentResult = new AssignmentResult
                    {
                        AssignmentId = assignmentId,
                    };

                    foreach (var (assignmentTaskId, grade) in assignmentTaskResults)
                    {
                        var assignmentTaskResult = new AssignmentTaskResult
                        {
                            AssignmentTaskId = assignmentTaskId,
                            Grade = grade
                        };

                        assignmentResult.AssignmentTaskResults.Add(assignmentTaskResult);
                    }

                    courseResult.AssignmentResults.Add(assignmentResult);
                }

                var existingStudent = await _context.Students.FindAsync(student.Id);

                if (existingStudent == null)
                {
                    _context.Students.Add(student);
                }
                else
                {
                    _context.Entry(existingStudent).CurrentValues.SetValues(student);
                }

                var existingCourseResult = await _context.CourseResults.FindAsync(courseResult.Id);

                if (existingCourseResult == null)
                {
                    _context.CourseResults.Add(courseResult);
                }
                else
                {
                    _context.Entry(existingCourseResult).CurrentValues.SetValues(courseResult);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}