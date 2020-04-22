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
    [ODataRoutePrefix("Course/{courseId}/Assignments/{assignmentId}/AssignmentTasks")]
    public class AssignmentTaskController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public AssignmentTaskController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Course/5/Assignments/10/AssignmentTasks
        [EnableQuery]
        [ODataRoute("")]
        public IQueryable<AssignmentTask> Get([FromODataUri] int assignmentId)
        {
            return _context.AssignmentTasks.Where(x => x.AssignmentId == assignmentId);
        }

        // GET: api/Course/5/Assignments/10/AssignmentTasks/15
        [EnableQuery]
        [ODataRoute("{id}")]
        public SingleResult<AssignmentTask> Get([FromODataUri] int id, [FromODataUri] int assignmentId)
        {
            IQueryable<AssignmentTask> assignmentTask = _context.AssignmentTasks.Where(x => x.Id == id && x.AssignmentId == assignmentId);

            return SingleResult.Create(assignmentTask);
        }

        // PUT: api/Course/5/Assignments/10/AssignmentTasks/15
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int assignmentId, [FromODataUri] int id, [FromBody] AssignmentTask assignmentTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != assignmentTask.Id || assignmentId != assignmentTask.AssignmentId)
            {
                return BadRequest();
            }

            _context.Entry(assignmentTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentTaskExists(id))
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

        // POST: api/Course/5/Assignments/10/AssignmentTasks/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("")]
        public async Task<ActionResult<AssignmentTask>> Post([FromODataUri] int courseId, [FromODataUri] int assignmentId, [FromBody] AssignmentTask assignmentTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            assignmentTask.AssignmentId = assignmentId;
            _context.AssignmentTasks.Add(assignmentTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { odataPath = $"Course/{courseId}/Assignments/{assignmentId}/AssignmentTasks/{assignmentTask.Id}" }, assignmentTask);
        }

        // DELETE: api/Course/5/Assignments/10/AssignmentTasks/15
        [ODataRoute("{id}")]
        public async Task<ActionResult<AssignmentTask>> Delete([FromODataUri] int assignmentId, [FromODataUri] int id)
        {
            var assignmentTask = await _context.AssignmentTasks.FirstOrDefaultAsync(x => x.Id == id && x.AssignmentId == assignmentId);
            if (assignmentTask == null)
            {
                return NotFound();
            }

            _context.AssignmentTasks.Remove(assignmentTask);
            await _context.SaveChangesAsync();

            return assignmentTask;
        }

        private bool AssignmentTaskExists(int id)
        {
            return _context.AssignmentTasks.Any(e => e.Id == id);
        }
    }
}
