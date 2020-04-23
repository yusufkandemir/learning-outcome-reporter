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
    [ODataRoutePrefix("CourseInfo/{courseInfoId}/Outcomes")]
    public class LearningOutcomeController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public LearningOutcomeController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/CourseInfo/5/Outcomes/
        [EnableQuery]
        [ODataRoute("")]
        public IQueryable<LearningOutcome> Get([FromODataUri] int courseInfoId)
        {
            return _context.Outcomes.OfType<LearningOutcome>().Where(x => x.CourseInfoId == courseInfoId);
        }

        // GET: api/CourseInfo/5/Outcomes/1
        [EnableQuery]
        [ODataRoute("{id}")]
        public SingleResult<LearningOutcome> Get([FromODataUri] int id, [FromODataUri] int courseInfoId)
        {
            IQueryable<LearningOutcome> outcome = _context.Outcomes.OfType<LearningOutcome>().Where(x => x.Id == id && x.CourseInfoId == courseInfoId);

            return SingleResult.Create(outcome);
        }

        // PUT: api/CourseInfo/5/Outcomes/10
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int courseInfoId, [FromODataUri] int id, [FromBody] LearningOutcome outcome)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != outcome.Id || courseInfoId != outcome.CourseInfoId)
            {
                return BadRequest();
            }

            _context.Entry(outcome).State = EntityState.Modified;

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

        // POST: api/CourseInfo/5/Outcomes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("")]
        public async Task<ActionResult<LearningOutcome>> Post([FromODataUri] int courseInfoId, [FromBody] LearningOutcome outcome)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            outcome.CourseInfoId = courseInfoId;
            _context.Outcomes.Add(outcome);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { odataPath = $"CourseInfo/{courseInfoId}/Outcomes/{outcome.Id}" }, outcome);
        }

        // DELETE: api/CourseInfo/5/Outcomes/10
        [ODataRoute("{id}")]
        public async Task<ActionResult<LearningOutcome>> Delete([FromODataUri] int courseInfoId, [FromODataUri] int id)
        {
            var outcome = await _context.Outcomes.OfType<LearningOutcome>().FirstOrDefaultAsync(x => x.Id == id && x.CourseInfoId == courseInfoId);
            if (outcome == null)
            {
                return NotFound();
            }

            _context.Outcomes.Remove(outcome);
            await _context.SaveChangesAsync();

            return outcome;
        }

        private bool AssignmentExists(int id)
        {
            return _context.Outcomes.OfType<LearningOutcome>().Any(e => e.Id == id);
        }
    }
}
