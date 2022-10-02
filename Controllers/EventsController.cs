using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using COMP2084_Project_Eventour.Data;
using COMP2084_Project_Eventour.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace COMP2084_Project_Eventour.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Events
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Events.Include(
            //    @ => @.EventDetail).Include(@ => @.User
            //    );


            //var applicationDbContext = from e in _context.Events join ed in _context.EventDetails on e.EventDetailId equals ed.EventDetailId join ev in _context.EventVenues on ed.EventVenueId equals ev.EventVenueId select new
            //{
            //    Title = e.Title,
            //    Description = e.Description,
            //    Address = ev.Address,
            //    CreatedOn = e.createdOn,

            //};

            // This works and joins the tables.
            var applicationDbContext = _context.Events
                .Include(x => x.User)
                .Include(x => x.EventDetail)
                .ThenInclude(x => x.Category)
                .Include(x => x.EventDetail)
                .ThenInclude(x => x.EventVenue);
                

            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            //var @event = await _context.Events
            //    .Include(@ => @.EventDetail)
            //    .Include(@ => @.User)
            //    .FirstOrDefaultAsync(m => m.EventId == id);

            var @event = await _context.Events
               .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
           ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
           //public List<EventVenue> EventTypes { get; set; } = new List<EventVenue>
           // {
           // };

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(
            "EventId,Title, Description,EventDetailId,UserId,createdOn"
            )] Event @event)
        {
            try
            {
                // Create the Venue into the database and return the ID.
                //string VenueType = @event.EventDetail.EventVenue.Type;
                string Address = HttpContext.Request.Form["EventDetail.EventVenue.Address"];
                string City = HttpContext.Request.Form["EventDetail.EventVenue.City"];
                string State = HttpContext.Request.Form["EventDetail.EventVenue.State"];
                string Country = HttpContext.Request.Form["EventDetail.EventVenue.Country"];
                string Zip = HttpContext.Request.Form["EventDetail.EventVenue.Zip"];

                // need to get event detail id first then add to event.
                var EventVenueId = new EventVenuesController(_context).CreateVenue(Address, City, State, Country, Zip);

                // Create the Details into the database and return the ID.
                double Price = Double.Parse(HttpContext.Request.Form["EventDetail.Price"]);
                DateTime StartDate = DateTime.Parse(HttpContext.Request.Form["EventDetail.StartDate"]);
                DateTime EndDate = DateTime.Parse(HttpContext.Request.Form["EventDetail.EndDate"]);
                string Photo = HttpContext.Request.Form["EventDetail.Photo"];
                int CategoryId = int.Parse(HttpContext.Request.Form["EventDetail.CategoryId"]);

                // Builds the event details and returns it ID.
                var EventDetailId = new EventDetailsController(_context).CreateDetail(Price, StartDate, EndDate, Photo, CategoryId, EventVenueId);

                @event.EventDetailId = EventDetailId;
                @event.UserId = 1;
                @event.createdOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    _context.Add(@event);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            ViewData["EventDetailId"] = new SelectList(_context.EventDetails, "EventDetailId", "EventDetailId", @event.EventDetailId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", @event.UserId);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["EventDetailId"] = new SelectList(_context.EventDetails, "EventDetailId", "EventDetailId", @event.EventDetailId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", @event.UserId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,EventDetailId,UserId,createdOn")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
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
            ViewData["EventDetailId"] = new SelectList(_context.EventDetails, "EventDetailId", "EventDetailId", @event.EventDetailId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", @event.UserId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            //var @event = await _context.Events
            //    .Include(@ => @.EventDetail)
            //    .Include(@ => @.User)
            //    .FirstOrDefaultAsync(m => m.EventId == id);

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Events'  is null.");
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
          return (_context.Events?.Any(e => e.EventId == id)).GetValueOrDefault();
        }
    }
}
