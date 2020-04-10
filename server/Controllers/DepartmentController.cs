using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Results;

namespace server.Controllers
{
    public class DepartmentController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public DepartmentController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Department
        [EnableQuery]
        public IQueryable<Department> Get()
        {
            return  _context.Departments;
        }

        // GET: api/Department/5
        [EnableQuery]
        public SingleResult<Department> Get([FromODataUri] int key)
        {
            IQueryable<Department> department =  _context.Departments.Where(x => x.Id == key);

            return SingleResult.Create(department);
        }

        // PUT: api/Department/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] Department department)
        {
            if (key != department.Id)
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
                if (!DepartmentExists(key))
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

        // POST: api/Department
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<CreatedODataResult<Department>> Post([FromBody] Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return Created(department);
        }

        // DELETE: api/Department/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> Delete(int key)
        {
            var department = await _context.Departments.FindAsync(key);
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
