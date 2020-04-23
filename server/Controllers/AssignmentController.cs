using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;

using server.Models;

namespace server.Controllers
{
    [ODataRoutePrefix("Course/{courseId}/Assignments")]
    public class AssignmentController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public AssignmentController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Course/5/Assignments/
        [EnableQuery]
        [ODataRoute("")]
        public IQueryable<Assignment> Get([FromODataUri] int courseId)
        {
            return _context.Assignments.Where(x => x.CourseId == courseId);
        }

        // GET: api/Course/5/Assignments/1
        [EnableQuery]
        [ODataRoute("{id}")]
        public SingleResult<Assignment> Get([FromODataUri] int id, [FromODataUri] int courseId)
        {
            IQueryable<Assignment> assignment = _context.Assignments.Where(x => x.Id == id && x.CourseId == courseId);

            return SingleResult.Create(assignment);
        }

        // PUT: api/Course/5/Assignments/10
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int courseId, [FromODataUri] int id, [FromBody] Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != assignment.Id || courseId != assignment.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Course/5/Assignments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("")]
        public async Task<ActionResult<Assignment>> Post([FromODataUri] int courseId, [FromBody] Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            assignment.CourseId = courseId;
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { odataPath = $"Course/{courseId}/Assignments/{assignment.Id}" }, assignment);
        }

        // DELETE: api/Course/5/Assignments/10
        [ODataRoute("{id}")]
        public async Task<ActionResult<Assignment>> DeleteAssignment([FromODataUri] int courseId, [FromODataUri] int id)
        {
            var assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.Id == id && x.CourseId == courseId);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return assignment;
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}
