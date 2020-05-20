using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData;

using server.Models;

namespace server.Controllers
{
    [Route("api/Courses/{courseId}/Assignments/{assignmentId}/AssignmentTasks")]
    [ApiController]
    public class AssignmentTaskController : ControllerBase
    {
        private readonly OutcomeReportingContext _context;

        public AssignmentTaskController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Courses/5/Assignments/10/AssignmentTasks
        [HttpGet]
        [EnableQuery]
        public IQueryable<AssignmentTask> GetAll([FromODataUri] int assignmentId)
        {
            return _context.AssignmentTasks.Where(x => x.AssignmentId == assignmentId);
        }

        // GET: api/Courses/5/Assignments/10/AssignmentTasks/15
        [HttpGet("{id}")]
        [EnableQuery]
        public SingleResult<AssignmentTask> Get([FromODataUri] int id, [FromODataUri] int assignmentId)
        {
            IQueryable<AssignmentTask> assignmentTask = _context.AssignmentTasks.Where(x => x.Id == id && x.AssignmentId == assignmentId);

            return SingleResult.Create(assignmentTask);
        }

        // PUT: api/Courses/5/Assignments/10/AssignmentTasks/15
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
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

        // POST: api/Courses/5/Assignments/10/AssignmentTasks/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<AssignmentTask>> Post([FromODataUri] int courseId, [FromODataUri] int assignmentId, [FromBody] AssignmentTask assignmentTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            assignmentTask.AssignmentId = assignmentId;
            _context.AssignmentTasks.Add(assignmentTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { courseId = courseId, assignmentId = assignmentId, id = assignmentTask.Id }, assignmentTask);
        }

        // DELETE: api/Courses/5/Assignments/10/AssignmentTasks/15
        [HttpDelete("{id}")]
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

        // PUT: api/Courses/5/Assignments/10/AssignmentTasks/15/Outcomes/20
        [HttpPut("{taskId}/Outcomes/{outcomeId}")]
        public async Task<ActionResult> AttachOutcome([FromRoute] int taskId, [FromRoute] int outcomeId)
        {
            var assignmentTask = await _context.AssignmentTasks.FindAsync(taskId);
            if (assignmentTask == null)
                return NotFound();

            var outcome = await _context.Outcomes.FindAsync(outcomeId);
            if (outcome == null)
                return NotFound();

            var assignmentTaskOutcome = new AssignmentTaskOutcome
            {
                AssignmentTaskId = taskId,
                OutcomeId = outcomeId
            };

            _context.AssignmentTaskOutcome.Add(assignmentTaskOutcome);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Courses/5/Assignments/10/AssignmentTasks/15/Outcomes/20
        [HttpDelete("{taskId}/Outcomes/{outcomeId}")]
        public async Task<ActionResult<AssignmentTaskOutcome>> DetachOutcome([FromRoute] int taskId, [FromRoute] int outcomeId)
        {
            var assignmentTaskOutcome = await _context.AssignmentTaskOutcome.FirstOrDefaultAsync(x => x.AssignmentTaskId == taskId && x.OutcomeId == outcomeId);
            if (assignmentTaskOutcome == null)
                return NotFound();

            _context.AssignmentTaskOutcome.Remove(assignmentTaskOutcome);
            await _context.SaveChangesAsync();

            return assignmentTaskOutcome;
        }

        private bool AssignmentTaskExists(int id)
        {
            return _context.AssignmentTasks.Any(e => e.Id == id);
        }
    }
}
