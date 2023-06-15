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
    public class EmployeesController : Controller
    {
        private readonly AdventureWorks2019Context _context;

        public EmployeesController(AdventureWorks2019Context context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string sortOrder,
                                               string currentFilter,
                                               string searchString,
                                               int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["JobSortParm"] = String.IsNullOrEmpty(sortOrder) ? "job_desc" : "";
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

            var employees = from e in _context.Employees
                           select e;
            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.JobTitle.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "job_desc":
                    employees = employees.OrderByDescending(e => e.JobTitle);
                    break;
                case "Date":
                    employees = employees.OrderBy(e => e.HireDate);
                    break;
                case "date_desc":
                    employees = employees.OrderByDescending(e => e.HireDate);
                    break;
                default:
                    employees = employees.OrderBy(e => e.NationalIdnumber);
                    break;
            }

            int pageSize = 25;
            return View(await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.BusinessEntity)
                .FirstOrDefaultAsync(m => m.BusinessEntityId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["BusinessEntityId"] = new SelectList(_context.People, "BusinessEntityId", "BusinessEntityId");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusinessEntityId,NationalIdnumber,LoginId,OrganizationLevel,JobTitle,BirthDate,MaritalStatus,Gender,HireDate,SalariedFlag,VacationHours,SickLeaveHours,CurrentFlag,Rowguid,ModifiedDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusinessEntityId"] = new SelectList(_context.People, "BusinessEntityId", "BusinessEntityId", employee.BusinessEntityId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["BusinessEntityId"] = new SelectList(_context.People, "BusinessEntityId", "BusinessEntityId", employee.BusinessEntityId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BusinessEntityId,NationalIdnumber,LoginId,OrganizationLevel,JobTitle,BirthDate,MaritalStatus,Gender,HireDate,SalariedFlag,VacationHours,SickLeaveHours,CurrentFlag,Rowguid,ModifiedDate")] Employee employee)
        {
            if (id != employee.BusinessEntityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.BusinessEntityId))
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
            ViewData["BusinessEntityId"] = new SelectList(_context.People, "BusinessEntityId", "BusinessEntityId", employee.BusinessEntityId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.BusinessEntity)
                .FirstOrDefaultAsync(m => m.BusinessEntityId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'AdventureWorks2019Context.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return (_context.Employees?.Any(e => e.BusinessEntityId == id)).GetValueOrDefault();
        }
    }
}
