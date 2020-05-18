using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Models;
using Microsoft.AspNet.OData;

namespace server.Controllers
{
    [Route("api/Departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly OutcomeReportingContext _context;

        public DepartmentController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        [EnableQuery]
        public IQueryable<Department> GetAll()
        {
            return _context.Departments;
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        [EnableQuery]
        public SingleResult<Department> Get([FromODataUri] int id)
        {
            IQueryable<Department> department = _context.Departments.Where(x => x.Id == id);

            return SingleResult.Create(department);
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int id, [FromBody] Department department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        // POST: api/Departments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Department>> Post([FromBody] Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = department.Id }, department);
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> Delete(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return department;
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
