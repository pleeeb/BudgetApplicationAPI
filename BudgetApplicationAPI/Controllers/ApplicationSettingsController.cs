using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetApplicationAPI.Models;

namespace BudgetApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationSettingsController : ControllerBase
    {
        private readonly BudgetContext _context;

        public ApplicationSettingsController(BudgetContext context)
        {
            _context = context;
        }

        // GET: api/ApplicationSettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationSetting>>> GetApplicationSetting()
        {
            var appSettings = await _context.ApplicationSetting.ToListAsync();
            if (appSettings == null || appSettings.Count == 0)
            {
                return NotFound();
            }
            return await _context.ApplicationSetting.ToListAsync();
        }

        // GET: api/ApplicationSettings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationSetting>> GetApplicationSetting(int id)
        {
            var appSettings = await _context.ApplicationSetting.ToListAsync();
            if (appSettings == null || appSettings.Count == 0)
            {
                return NotFound();
            }
            var applicationSetting = await _context.ApplicationSetting.FindAsync(id);

            if (applicationSetting == null)
            {
                return NotFound();
            }

            return applicationSetting;
        }

        // PUT: api/ApplicationSettings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationSetting(int id, ApplicationSetting applicationSetting)
        {
            if (id != applicationSetting.SettingId)
            {
                return BadRequest();
            }

            _context.Entry(applicationSetting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationSettingExists(id))
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

        // POST: api/ApplicationSettings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApplicationSetting>> PostApplicationSetting(ApplicationSetting applicationSetting)
        {
            if (_context.ApplicationSetting == null)
            {
                return Problem("Entity set 'BudgetContext.ApplicationSetting'  is null.");
            }
            _context.ApplicationSetting.Add(applicationSetting);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApplicationSetting", new { id = applicationSetting.SettingId }, applicationSetting);
        }

        // DELETE: api/ApplicationSettings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplicationSetting(int id)
        {
            if (_context.ApplicationSetting == null)
            {
                return NotFound();
            }
            var applicationSetting = await _context.ApplicationSetting.FindAsync(id);
            if (applicationSetting == null)
            {
                return NotFound();
            }

            _context.ApplicationSetting.Remove(applicationSetting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicationSettingExists(int id)
        {
            return (_context.ApplicationSetting?.Any(e => e.SettingId == id)).GetValueOrDefault();
        }
    }
}
