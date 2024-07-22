using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models; // Update with your actual namespace
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Controllers // Update with your actual namespace
{
    public class InsureeController : Controller
    {
        private readonly YourDbContext _context;

        public InsureeController(YourDbContext context)
        {
            _context = context;
        }

        // GET: Insuree/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,SpeedingTickets,IsDUI,IsFullCoverage")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                insuree.Quote = CalculateQuote(insuree);
                _context.Add(insuree);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(insuree);
        }

        private decimal CalculateQuote(Insuree insuree)
        {
            decimal baseQuote = 50;

            // Age calculation
            var age = DateTime.Now.Year - insuree.DateOfBirth.Year;
            if (DateTime.Now.DayOfYear < insuree.DateOfBirth.DayOfYear)
                age--;

            if (age <= 18)
                baseQuote += 100;
            else if (age >= 19 && age <= 25)
                baseQuote += 50;
            else
                baseQuote += 25;

            // Car Year
            if (insuree.CarYear < 2000)
                baseQuote += 25;
            if (insuree.CarYear > 2015)
                baseQuote += 25;

            // Car Make and Model
            if (insuree.CarMake.ToLower() == "porsche")
            {
                baseQuote += 25;
                if (insuree.CarModel.ToLower() == "911 carrera")
                    baseQuote += 25;
            }

            // Speeding Tickets
            baseQuote += insuree.SpeedingTickets * 10;

            // DUI
            if (insuree.IsDUI)
                baseQuote *= 1.25m;

            // Full Coverage
            if (insuree.IsFullCoverage)
                baseQuote *= 1.5m;

            return baseQuote;
        }

        public async Task<IActionResult> Admin()
        {
            return View(await _context.Insurees.ToListAsync());
        }
    }
}
