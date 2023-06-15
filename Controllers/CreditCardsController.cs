using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdventureWorks.Models;

namespace AdventureWorks.Controllers
{
    public class CreditCardsController : Controller
    {
        private readonly AdventureWorks2019Context _context;

        public CreditCardsController(AdventureWorks2019Context context)
        {
            _context = context;
        }

        // GET: CreditCards
        public async Task<IActionResult> Index()
        {
              return _context.CreditCards != null ? 
                          View(await _context.CreditCards.ToListAsync()) :
                          Problem("Entity set 'AdventureWorks2019Context.CreditCards'  is null.");
        }

        // GET: CreditCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CreditCards == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCards
                .FirstOrDefaultAsync(m => m.CreditCardId == id);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // GET: CreditCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CreditCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreditCardId,CardType,CardNumber,ExpMonth,ExpYear,ModifiedDate")] CreditCard creditCard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(creditCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(creditCard);
        }

        // GET: CreditCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CreditCards == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCards.FindAsync(id);
            if (creditCard == null)
            {
                return NotFound();
            }
            return View(creditCard);
        }

        // POST: CreditCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CreditCardId,CardType,CardNumber,ExpMonth,ExpYear,ModifiedDate")] CreditCard creditCard)
        {
            if (id != creditCard.CreditCardId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creditCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditCardExists(creditCard.CreditCardId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(creditCard);
        }

        // GET: CreditCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CreditCards == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCards
                .FirstOrDefaultAsync(m => m.CreditCardId == id);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // POST: CreditCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CreditCards == null)
            {
                return Problem("Entity set 'AdventureWorks2019Context.CreditCards'  is null.");
            }
            var creditCard = await _context.CreditCards.FindAsync(id);
            if (creditCard != null)
            {
                _context.CreditCards.Remove(creditCard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditCardExists(int id)
        {
          return (_context.CreditCards?.Any(e => e.CreditCardId == id)).GetValueOrDefault();
        }
    }
}
