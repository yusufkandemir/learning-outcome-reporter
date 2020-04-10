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
    public class StudentController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public StudentController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/Student
        [EnableQuery]
        public IQueryable<Student> Get()
        {
            return  _context.Students;
        }

        // GET: api/Student/5
        [EnableQuery]
        public SingleResult<Student> Get([FromODataUri] string key)
        {
            IQueryable<Student> student =  _context.Students.Where(x => x.Id == key);

            return SingleResult.Create(student);
        }

        // PUT: api/Student/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromODataUri] string key, [FromBody] Student student)
        {
            if (key != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(key))
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

        // POST: api/Student
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<CreatedODataResult<Student>> Post([FromBody] Student student)
        {
            _context.Students.Add(student);
            
            await _context.SaveChangesAsync();

            return Created(student);
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> Delete(string key)
        {
            var student = await _context.Students.FindAsync(key);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return student;
        }

        private bool StudentExists(string id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
