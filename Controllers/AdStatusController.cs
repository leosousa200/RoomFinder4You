using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomFinder4You.Data;
using RoomFinder4You.Models;

namespace RoomFinder4You
{
    [Authorize(Roles ="Admin")]
    public class AdStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdStatus
        public async Task<IActionResult> Index()
        {
              return _context.AdsStatus != null ? 
                          View(await _context.AdsStatus.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.AdsStatus'  is null.");
        }

        // GET: AdStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AdsStatus == null)
            {
                return NotFound();
            }

            var adStatus = await _context.AdsStatus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adStatus == null)
            {
                return NotFound();
            }

            return View(adStatus);
        }

        // GET: AdStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status")] AdStatus adStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adStatus);
        }

        // GET: AdStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AdsStatus == null)
            {
                return NotFound();
            }

            var adStatus = await _context.AdsStatus.FindAsync(id);
            if (adStatus == null)
            {
                return NotFound();
            }
            return View(adStatus);
        }

        // POST: AdStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] AdStatus adStatus)
        {
            if (id != adStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdStatusExists(adStatus.Id))
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
            return View(adStatus);
        }

        // GET: AdStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AdsStatus == null)
            {
                return NotFound();
            }

            var adStatus = await _context.AdsStatus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adStatus == null)
            {
                return NotFound();
            }

            return View(adStatus);
        }

        // POST: AdStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AdsStatus == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AdsStatus'  is null.");
            }
            var adStatus = await _context.AdsStatus.FindAsync(id);
            if (adStatus != null)
            {
                _context.AdsStatus.Remove(adStatus);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdStatusExists(int id)
        {
          return (_context.AdsStatus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
