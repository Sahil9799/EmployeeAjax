using Ajax_EmployeePayroll.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Ajax_EmployeePayroll.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeDbContext _context;
            
        public EmployeeController(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEmployee(int id = 0)
        {
            if (id == 0)
                return View(new EmployeeModel());
            else
            {
                var empl = await _context.Employees.FindAsync(id);
                if (empl == null)
                {
                    return NotFound();
                }
                return View(empl);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(int id, [Bind("Emp_Id,Name,Gender,Department,Notes")] EmployeeModel emps)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (id == 0)
                {

                    _context.Add(emps);
                    await _context.SaveChangesAsync();

                }
                //Update
                else
                {
                    try
                    {
                        _context.Update(emps);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmployeeModelExists(emps.Emp_Id))
                        { return NotFound(); }
                        else
                        { throw; }
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Employees.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddEmployee", emps) });
        }


        // GET: Employee/DeleteEmployee
        public async Task<IActionResult> DeleteEmployee(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empl = await _context.Employees
                .FirstOrDefaultAsync(m => m.Emp_Id == id);
            if (empl == null)
            {
                return NotFound();
            }

            return View(empl);
        }

        // POST: Employee/DeleteEmployee/5
        [HttpPost, ActionName("DeleteEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empl = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(empl);
            await _context.SaveChangesAsync();
            return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Employees.ToList()) });
        }

        private bool EmployeeModelExists(int id)
        {
            return _context.Employees.Any(e => e.Emp_Id == id);
        }
    }
}