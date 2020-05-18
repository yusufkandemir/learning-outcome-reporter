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
    [Route("api/CourseInfos/{courseInfoId}/Courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly OutcomeReportingContext _context;

        public CourseController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/CourseInfos/5/Courses/
        [HttpGet]
        [EnableQuery]
        public IQueryable<Course> GetAll([FromODataUri] int courseInfoId)
        {
            return _context.Courses.Where(x => x.CourseInfoId == courseInfoId);
        }

        // GET: api/CourseInfos/5/Courses/1
        [HttpGet("{id}")]
        [EnableQuery]
        public SingleResult<Course> Get([FromODataUri] int id, [FromODataUri] int courseInfoId)
        {
            IQueryable<Course> course = _context.Courses.Where(x => x.Id == id && x.CourseInfoId == courseInfoId);

            return SingleResult.Create(course);
        }

        // PUT: api/CourseInfos/5/Courses/10
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
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

        // POST: api/CourseInfos/5/Courses
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Course>> Post([FromODataUri] int courseInfoId, [FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            course.CourseInfoId = courseInfoId;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { courseInfoId = courseInfoId, id = course.Id }, course);
        }

        // DELETE: api/CourseInfos/5/Courses/10
        [HttpDelete("{id}")]
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
