using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoomFinder4You.Data;
using RoomFinder4You.Models;
using RoomFinder4You.ViewModels;

namespace RoomFinder4You
{
    public class AdController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Ad
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ads.Include(a => a.User).Include(a => a.adStatus).Include(a => a.room);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ads == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        // GET: Ad/Create
        public IActionResult Create()
        {
            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Status");
            ViewData["FeatureTypesMandatory"] = new SelectList(_context.FeatureTypes.Where(v => v.IsMandatory), "Sigla", "Name");
            ViewData["FeatureTypesNonMandatory"] = new SelectList(_context.FeatureTypes.Where(v => !v.IsMandatory), "Sigla", "Name");

            return View();
        }

        // POST: Ad/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,AdStatusId,room.Price")] Ad ad, float price)
        {

                ModelState.Remove(nameof(ad.room));
                ModelState.Remove(nameof(ad.User));
                ModelState.Remove(nameof(ad.UserID));
                ModelState.Remove(nameof(ad.adStatus));

            if (ModelState.IsValid)
            {
                // Room logic
                Room room = new Room();
                room.Features = new List<Feature>();
                room.Price = price;
                ad.room = room;

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ad.UserID = userId;
                _context.Add(ad);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Status");
            ViewData["FeatureTypesMandatory"] = new SelectList(_context.FeatureTypes.Where(v => v.IsMandatory), "Sigla", "Name");
            ViewData["FeatureTypesNonMandatory"] = new SelectList(_context.FeatureTypes.Where(v => !v.IsMandatory), "Sigla", "Name");

            return View(ad);
        }

        // GET: Ad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ads == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads.FindAsync(id);
            if (ad == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", ad.UserID);
            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Id", ad.AdStatusId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", ad.RoomId);
            return View(ad);
        }

        // POST: Ad/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,RoomId,AdStatusId,UserID")] Ad ad)
        {
            if (id != ad.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ad);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdExists(ad.Id))
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
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", ad.UserID);
            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Id", ad.AdStatusId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Id", ad.RoomId);
            return View(ad);
        }

        // GET: Ad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ads == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ad == null)
            {
                return NotFound();
            }

            return View(ad);
        }

        // POST: Ad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ads == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ads'  is null.");
            }
            var ad = await _context.Ads.FindAsync(id);
            if (ad != null)
            {
                _context.Ads.Remove(ad);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdExists(int id)
        {
          return (_context.Ads?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
