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
    [ODataRoutePrefix("Department/{departmentId}/Outcomes")]
    public class DepartmentOutcomeController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public DepartmentOutcomeController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Department/5/Outcomes/
        [EnableQuery]
        [ODataRoute("")]
        public IQueryable<ProgramOutcome> Get([FromODataUri] int departmentId)
        {
            return _context.Outcomes.OfType<ProgramOutcome>().Where(x => x.DepartmentId == departmentId);
        }

        // GET: api/Department/5/Outcomes/1
        [EnableQuery]
        [ODataRoute("{id}")]
        public SingleResult<ProgramOutcome> Get([FromODataUri] int id, [FromODataUri] int departmentId)
        {
            IQueryable<ProgramOutcome> outcome = _context.Outcomes.OfType<ProgramOutcome>().Where(x => x.Id == id && x.DepartmentId == departmentId);

            return SingleResult.Create(outcome);
        }

        // PUT: api/Department/5/Outcomes/10
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int departmentId, [FromODataUri] int id, [FromBody] ProgramOutcome outcome)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != outcome.Id || departmentId != outcome.DepartmentId)
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

        // POST: api/Department/5/Outcomes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("")]
        public async Task<ActionResult<ProgramOutcome>> Post([FromODataUri] int departmentId, [FromBody] ProgramOutcome outcome)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            outcome.DepartmentId = departmentId;
            _context.Outcomes.Add(outcome);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { odataPath = $"Department/{departmentId}/Outcomes/{outcome.Id}" }, outcome);
        }

        // DELETE: api/Department/5/Outcomes/10
        [ODataRoute("{id}")]
        public async Task<ActionResult<ProgramOutcome>> Delete([FromODataUri] int departmentId, [FromODataUri] int id)
        {
            var outcome = await _context.Outcomes.OfType<ProgramOutcome>().FirstOrDefaultAsync(x => x.Id == id && x.DepartmentId == departmentId);
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
            return _context.Outcomes.OfType<ProgramOutcome>().Any(e => e.Id == id);
        }
    }
}
