using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP2084_Project_Eventour.Data;
using COMP2084_Project_Eventour.Models;

namespace COMP2084_Project_Eventour.Controllers
{
    public class EventDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EventDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EventDetails.Include(e => e.Category).Include(e => e.EventVenue);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EventDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EventDetails == null)
            {
                return NotFound();
            }

            var eventDetail = await _context.EventDetails
                .Include(e => e.Category)
                .Include(e => e.EventVenue)
                .FirstOrDefaultAsync(m => m.EventDetailId == id);
            if (eventDetail == null)
            {
                return NotFound();
            }

            return View(eventDetail);
        }

        // GET: EventDetails/Create
        public IActionResult Create()
        {

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Description");
            ViewData["EventVenueId"] = new SelectList(_context.EventVenues, "EventVenueId", "Country");
            return View();
        }

        // POST: EventDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventDetailId,Price,StartDate,EndDate,CategoryId,EventVenueId")] EventDetail eventDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Description", eventDetail.CategoryId);
            ViewData["EventVenueId"] = new SelectList(_context.EventVenues, "EventVenueId", "Country", eventDetail.EventVenueId);
            return View(eventDetail);
        }


        [ValidateAntiForgeryToken]
        public int CreateDetail(double Price, DateTime StartDate, DateTime EndDate, int CategoryId, int EventVenueId)
        {
            try
            {
                EventDetail eventDetail = new EventDetail();

                eventDetail.Price = Price;
                eventDetail.StartDate = StartDate;
                eventDetail.EndDate = EndDate;
                eventDetail.CategoryId = CategoryId;
                eventDetail.EventVenueId = EventVenueId;

                if(ModelState.IsValid)
                {
                    _context.Add(eventDetail);
                    _context.SaveChanges();

                    return eventDetail.EventDetailId;
                }

            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
           

            return 0;
        }



        // GET: EventDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EventDetails == null)
            {
                return NotFound();
            }

            var eventDetail = await _context.EventDetails.FindAsync(id);
            if (eventDetail == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Description", eventDetail.CategoryId);
            ViewData["EventVenueId"] = new SelectList(_context.EventVenues, "EventVenueId", "Country", eventDetail.EventVenueId);
            return View(eventDetail);
        }

        // POST: EventDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventDetailId,Price,StartDate,EndDate,Photo,CategoryId,EventVenueId")] EventDetail eventDetail)
        {
            if (id != eventDetail.EventDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventDetailExists(eventDetail.EventDetailId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Description", eventDetail.CategoryId);
            ViewData["EventVenueId"] = new SelectList(_context.EventVenues, "EventVenueId", "Country", eventDetail.EventVenueId);
            return View(eventDetail);
        }

        // GET: EventDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EventDetails == null)
            {
                return NotFound();
            }

            var eventDetail = await _context.EventDetails
                .Include(e => e.Category)
                .Include(e => e.EventVenue)
                .FirstOrDefaultAsync(m => m.EventDetailId == id);
            if (eventDetail == null)
            {
                return NotFound();
            }

            return View(eventDetail);
        }

        // POST: EventDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EventDetails == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EventDetails'  is null.");
            }
            var eventDetail = await _context.EventDetails.FindAsync(id);
            if (eventDetail != null)
            {
                _context.EventDetails.Remove(eventDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventDetailExists(int id)
        {
          return (_context.EventDetails?.Any(e => e.EventDetailId == id)).GetValueOrDefault();
        }
    }
}
