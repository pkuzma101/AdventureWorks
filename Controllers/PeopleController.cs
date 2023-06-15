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
    public class PeopleController : Controller
    {
        private readonly AdventureWorks2019Context _context;

        public PeopleController(AdventureWorks2019Context context)
        {
            _context = context;
        }

        // GET: People
        public async Task<IActionResult> Index(string sortOrder,
                                               string currentFilter,
                                               string searchString,
                                               int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "last_name" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var people = from p in _context.People
                            select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                people = people.Where(p => p.FirstName.Contains(searchString) || p.LastName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "last_name":
                    people = people.OrderByDescending(p => p.LastName);
                    break;
                case "Date":
                    people = people.OrderBy(p => p.ModifiedDate);
                    break;
                case "date_desc":
                    people = people.OrderByDescending(p => p.ModifiedDate);
                    break;
                default:
                    people = people.OrderBy(p => p.BusinessEntityId);
                    break;
            }

            int pageSize = 25;
            return View(await PaginatedList<Person>.CreateAsync(people.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.People == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .Include(p => p.BusinessEntity)
                .FirstOrDefaultAsync(m => m.BusinessEntityId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            ViewData["BusinessEntityId"] = new SelectList(_context.BusinessEntities, "BusinessEntityId", "BusinessEntityId");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusinessEntityId,PersonType,NameStyle,Title,FirstName,MiddleName,LastName,Suffix,EmailPromotion,AdditionalContactInfo,Demographics,Rowguid,ModifiedDate")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusinessEntityId"] = new SelectList(_context.BusinessEntities, "BusinessEntityId", "BusinessEntityId", person.BusinessEntityId);
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.People == null)
            {
                return NotFound();
            }

            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            ViewData["BusinessEntityId"] = new SelectList(_context.BusinessEntities, "BusinessEntityId", "BusinessEntityId", person.BusinessEntityId);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BusinessEntityId,PersonType,NameStyle,Title,FirstName,MiddleName,LastName,Suffix,EmailPromotion,AdditionalContactInfo,Demographics,Rowguid,ModifiedDate")] Person person)
        {
            if (id != person.BusinessEntityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.BusinessEntityId))
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
            ViewData["BusinessEntityId"] = new SelectList(_context.BusinessEntities, "BusinessEntityId", "BusinessEntityId", person.BusinessEntityId);
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.People == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .Include(p => p.BusinessEntity)
                .FirstOrDefaultAsync(m => m.BusinessEntityId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.People == null)
            {
                return Problem("Entity set 'AdventureWorks2019Context.People'  is null.");
            }
            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
          return (_context.People?.Any(e => e.BusinessEntityId == id)).GetValueOrDefault();
        }
    }
}
