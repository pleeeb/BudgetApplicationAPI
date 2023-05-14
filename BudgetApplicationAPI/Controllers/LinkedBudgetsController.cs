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
    public class LinkedBudgetsController : ControllerBase
    {
        private readonly IBudgetContext _context;

        public LinkedBudgetsController(IBudgetContext context)
        {
            _context = context;
        }

        // GET: api/LinkedBudgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LinkedBudget>>> GetLinkedBudget()
        {
            var linkedBudget = await _context.LinkedBudget.ToListAsync().ConfigureAwait(false);
            if (linkedBudget == null || linkedBudget.Count == 0)
            {
                return NotFound();
            }
            return linkedBudget;
        }

        // GET: api/LinkedBudgets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LinkedBudget>> GetLinkedBudget(int id)
        {
            if (_context.LinkedBudget == null)
            {
                return NotFound();
            }
            var linkedBudget = await _context.LinkedBudget.FindAsync(id).ConfigureAwait(false);

            if (linkedBudget == null)
            {
                return NotFound();
            }

            return linkedBudget;
        }

        // PUT: api/LinkedBudgets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLinkedBudget(int id, LinkedBudget linkedBudget)
        {
            if (id != linkedBudget.LinkId)
            {
                return BadRequest();
            }

            _context.Entry(linkedBudget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LinkedBudgetExists(id))
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

        // POST: api/LinkedBudgets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LinkedBudget>> PostLinkedBudget(LinkedBudget linkedBudget)
        {
            if (_context.LinkedBudget == null)
            {
                return Problem("Entity set 'BudgetContext.LinkedBudget'  is null.");
            }
            _context.LinkedBudget.Add(linkedBudget);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return CreatedAtAction("GetLinkedBudget", new { id = linkedBudget.LinkId }, linkedBudget);
        }

        // DELETE: api/LinkedBudgets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLinkedBudget(int id)
        {
            if (_context.LinkedBudget == null)
            {
                return NotFound();
            }
            var linkedBudget = await _context.LinkedBudget.FindAsync(id).ConfigureAwait(false);
            if (linkedBudget == null)
            {
                return NotFound();
            }

            _context.LinkedBudget.Remove(linkedBudget);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return NoContent();
        }

        private bool LinkedBudgetExists(int id)
        {
            return (_context.LinkedBudget?.Any(e => e.LinkId == id)).GetValueOrDefault();
        }
    }
}
