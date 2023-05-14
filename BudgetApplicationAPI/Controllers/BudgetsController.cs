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
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetContext _context;

        public BudgetsController(IBudgetContext context)
        {
            _context = context;
        }

        // GET: api/Budgets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Budget>>> GetBudget()
        {
            var budget = await _context.Budget.ToListAsync().ConfigureAwait(false);
            if (budget == null || budget.Count == 0) 
            {
                return NotFound();
            }
            return budget;
        }

        // GET: api/Budgets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Budget>> GetBudget(int id)
        {
          if (_context.Budget == null)
          {
              return NotFound();
          }
            var budget = await _context.Budget.FindAsync(id).ConfigureAwait(false);

            if (budget == null)
            {
                return NotFound();
            }

            return budget;
        }

        // PUT: api/Budgets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBudget(int id, Budget budget)
        {
            if (id != budget.BudgetId)
            {
                return BadRequest();
            }

            _context.Entry(budget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BudgetExists(id))
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

        // POST: api/Budgets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Budget>> PostBudget(Budget budget)
        {
          if (_context.Budget == null)
          {
              return Problem("Entity set 'BudgetContext.Budget'  is null.");
          }
            _context.Budget.Add(budget);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return CreatedAtAction("GetBudget", new { id = budget.BudgetId }, budget);
        }

        // DELETE: api/Budgets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            if (_context.Budget == null)
            {
                return NotFound();
            }
            var budget = await _context.Budget.FindAsync(id).ConfigureAwait(false);
            if (budget == null)
            {
                return NotFound();
            }

            _context.Budget.Remove(budget);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return NoContent();
        }

        private bool BudgetExists(int id)
        {
            return (_context.Budget?.Any(e => e.BudgetId == id)).GetValueOrDefault();
        }
    }
}
