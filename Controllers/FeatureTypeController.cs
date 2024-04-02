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
    public class FeatureTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeatureTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FeatureType
        public async Task<IActionResult> Index()
        {
              return _context.FeatureTypes != null ? 
                          View(await _context.FeatureTypes.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.FeatureTypes'  is null.");
        }

        // GET: FeatureType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FeatureTypes == null)
            {
                return NotFound();
            }

            var featureType = await _context.FeatureTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (featureType == null)
            {
                return NotFound();
            }

            return View(featureType);
        }

        // GET: FeatureType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FeatureType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Initials,IsMandatory")] FeatureType featureType)
        {
            if (ModelState.IsValid )
            {
                // checks if is unique
                if(_context.FeatureTypes.Count(v => v.Initials.Equals(featureType.Initials)) != 0)
                    return View(featureType);

                _context.Add(featureType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(featureType);
        }

        // GET: FeatureType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FeatureTypes == null)
            {
                return NotFound();
            }

            var featureType = await _context.FeatureTypes.FindAsync(id);
            if (featureType == null)
            {
                return NotFound();
            }
            return View(featureType);
        }

        // POST: FeatureType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Initials,IsMandatory")] FeatureType featureType)
        {
            if (id != featureType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // checks if is unique
                if(_context.FeatureTypes.Count(v => v.Initials.Equals(featureType.Initials)) != 0)
                    return View(featureType);
                    
                try
                {
                    _context.Update(featureType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeatureTypeExists(featureType.Id))
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
            return View(featureType);
        }

        // GET: FeatureType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FeatureTypes == null)
            {
                return NotFound();
            }

            var featureType = await _context.FeatureTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (featureType == null)
            {
                return NotFound();
            }

            return View(featureType);
        }

        // POST: FeatureType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FeatureTypes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FeatureTypes'  is null.");
            }
            var featureType = await _context.FeatureTypes.FindAsync(id);
            if (featureType != null)
            {
                _context.FeatureTypes.Remove(featureType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeatureTypeExists(int id)
        {
          return (_context.FeatureTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
