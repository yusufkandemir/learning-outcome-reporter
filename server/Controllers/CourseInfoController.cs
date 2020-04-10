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
    public class CourseInfoController : ODataController
    {
        private readonly OutcomeReportingContext _context;

        public CourseInfoController(OutcomeReportingContext context)
        {
            _context = context;
        }

        // GET: api/CourseInfo
        [EnableQuery]
        public IQueryable<CourseInfo> Get()
        {
            return  _context.CourseInfos;
        }

        // GET: api/CourseInfo/5
        [EnableQuery]
        public SingleResult<CourseInfo> Get([FromODataUri] int key)
        {
            IQueryable<CourseInfo> courseInfo =  _context.CourseInfos.Where(x => x.Id == key);

            return SingleResult.Create(courseInfo);
        }

        // PUT: api/CourseInfo/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] CourseInfo courseInfo)
        {
            if (key != courseInfo.Id)
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
                if (!CourseInfoExists(key))
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

        // POST: api/CourseInfo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<CreatedODataResult<CourseInfo>> Post([FromBody] CourseInfo courseInfo)
        {
            _context.CourseInfos.Add(courseInfo);
            await _context.SaveChangesAsync();

            return Created(courseInfo);
        }

        // DELETE: api/CourseInfo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseInfo>> Delete(int key)
        {
            var courseInfo = await _context.CourseInfos.FindAsync(key);
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
