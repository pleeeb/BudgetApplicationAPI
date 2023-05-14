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
    public class AuthenticationsController : ControllerBase
    {
        private readonly IBudgetContext _context;

        public AuthenticationsController(IBudgetContext context)
        {
            _context = context;
        }

        // GET: api/Authentications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Authentication>>> GetAuthentication()
        {
            var authentications = await _context.Authentication.ToListAsync().ConfigureAwait(false);
            if (authentications == null || authentications.Count == 0)
            {
                return NotFound();
            }
            return authentications;
        }

        // GET: api/Authentications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Authentication>> GetAuthentication(string id)
        {
            var authentications = await _context.Authentication.ToListAsync().ConfigureAwait(false);
            if (authentications == null || authentications.Count == 0)
            {
                return NotFound();
            }
            var authentication = await _context.Authentication.FindAsync(id).ConfigureAwait(false);

            if (authentication == null)
            {
                return NotFound();
            }

            return authentication;
        }

        // PUT: api/Authentications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthentication(string id, Authentication authentication)
        {
            if (id != authentication.AuthToken)
            {
                return BadRequest();
            }

            _context.Entry(authentication).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthenticationExists(id))
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

        // POST: api/Authentications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Authentication>> PostAuthentication(Authentication authentication)
        {
            if (_context.Authentication == null)
            {
                return Problem("Entity set 'BudgetContext.Authentication'  is null.");
            }
            _context.Authentication.Add(authentication);
            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException)
            {
                if (authentication.AuthToken != null && AuthenticationExists(authentication.AuthToken))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAuthentication", new { id = authentication.AuthToken }, authentication);
        }

        // DELETE: api/Authentications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthentication(string id)
        {
            if (_context.Authentication == null)
            {
                return NotFound();
            }
            var authentication = await _context.Authentication.FindAsync(id).ConfigureAwait(false);
            if (authentication == null)
            {
                return NotFound();
            }

            _context.Authentication.Remove(authentication);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return NoContent();
        }

        private bool AuthenticationExists(string id)
        {
            return (_context.Authentication?.Any(e => e.AuthToken == id)).GetValueOrDefault();
        }
    }
}
