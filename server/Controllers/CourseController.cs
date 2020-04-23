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
    [ODataRoutePrefix("CourseInfo/{courseInfoId}/Courses")]
    public class CourseController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public CourseController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/CourseInfo/5/Courses/
        [EnableQuery]
        [ODataRoute("")]
        public IQueryable<Course> Get([FromODataUri] int courseInfoId)
        {
            return _context.Courses.Where(x => x.CourseInfoId == courseInfoId);
        }

        // GET: api/CourseInfo/5/Courses/1
        [EnableQuery]
        [ODataRoute("{id}")]
        public SingleResult<Course> Get([FromODataUri] int id, [FromODataUri] int courseInfoId)
        {
            IQueryable<Course> course = _context.Courses.Where(x => x.Id == id && x.CourseInfoId == courseInfoId);

            return SingleResult.Create(course);
        }

        // PUT: api/CourseInfo/5/Courses/10
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int courseInfoId, [FromODataUri] int id, [FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id || courseInfoId != course.CourseInfoId)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        // POST: api/CourseInfo/5/Courses
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ODataRoute("")]
        public async Task<ActionResult<Course>> Post([FromODataUri] int courseInfoId, [FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            course.CourseInfoId = courseInfoId;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { odataPath = $"CourseInfo/{courseInfoId}/Courses/{course.Id}" }, course);
        }

        // DELETE: api/CourseInfo/5/Courses/10
        [ODataRoute("{id}")]
        public async Task<ActionResult<Course>> DeleteCourse([FromODataUri] int courseInfoId, [FromODataUri] int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id && x.CourseInfoId == courseInfoId);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return course;
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
