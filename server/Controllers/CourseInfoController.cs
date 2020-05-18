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
    [Route("api/CourseInfos")]
    [ApiController]
    public class CourseInfoController : ControllerBase
    {
        private readonly OutcomeReportingContext _context;

        public CourseInfoController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/CourseInfos
        [HttpGet]
        [EnableQuery]
        public IQueryable<CourseInfo> GetAll()
        {
            return _context.CourseInfos;
        }

        // GET: api/CourseInfos/5
        [HttpGet("{id}")]
        [EnableQuery]
        public SingleResult<CourseInfo> Get([FromODataUri] int id)
        {
            IQueryable<CourseInfo> courseInfo = _context.CourseInfos.Where(x => x.Id == id);

            return SingleResult.Create(courseInfo);
        }

        // PUT: api/CourseInfos/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int id, [FromBody] CourseInfo courseInfo)
        {
            if (id != courseInfo.Id)
            {
                return BadRequest();
            }

            _context.Entry(courseInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseInfoExists(id))
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

        // POST: api/CourseInfos
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<CourseInfo>> Post([FromBody] CourseInfo courseInfo)
        {
            _context.CourseInfos.Add(courseInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = courseInfo.Id }, courseInfo);
        }

        // DELETE: api/CourseInfos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseInfo>> Delete(int id)
        {
            var courseInfo = await _context.CourseInfos.FindAsync(id);
            if (courseInfo == null)
            {
                return NotFound();
            }

            _context.CourseInfos.Remove(courseInfo);
            await _context.SaveChangesAsync();

            return courseInfo;
        }

        private bool CourseInfoExists(int id)
        {
            return _context.CourseInfos.Any(e => e.Id == id);
        }
    }
}
