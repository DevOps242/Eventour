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
    public class EventVenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventVenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EventVenues
        public async Task<IActionResult> Index()
        {
              return _context.EventVenues != null ? 
                          View(await _context.EventVenues.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.EventVenues'  is null.");
        }

        // GET: EventVenues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EventVenues == null)
            {
                return NotFound();
            }

            var eventVenue = await _context.EventVenues
                .FirstOrDefaultAsync(m => m.EventVenueId == id);
            if (eventVenue == null)
            {
                return NotFound();
            }

            return View(eventVenue);
        }

        // GET: EventVenues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EventVenues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventVenueId,Type,Country,City,State,Address,Zip")] EventVenue eventVenue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventVenue);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }



            return View(eventVenue);
        }

        /**
         * This method adds an Event Venue then return the ID for that venue added.
         * **/
        [ValidateAntiForgeryToken]
        public int CreateVenue(int eventTypeVal, string? Address, string? City, string? State, string? Country, string? Zip)
        {
            try
            {
                EventVenue eventVenue = new EventVenue();

                //eventVenue.EventType = EventVenue.EventType.Remote;
                eventVenue.Type = (EventVenue.EventType)eventTypeVal;
                eventVenue.Address = Address;
                eventVenue.City = City;
                eventVenue.State = State;
                eventVenue.Country = Country;
                eventVenue.Zip = Zip;

                if (ModelState.IsValid)
                {
                    _context.Add(eventVenue);
                    _context.SaveChanges();
                    //_context.SaveChangesAsync();

                    // Get the last id from the database and return it
                    return eventVenue.EventVenueId;
                }

            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
             

            return 0;
        }

        /**
         * This method adds an Event Venue then return the ID for that venue added.
         * **/
        [ValidateAntiForgeryToken]
        public int EditVenue(int venueID, int eventTypeVal, string? Address, string? City, string? State, string? Country, string? Zip)
        {
            try
            {
                EventVenue eventVenue = new EventVenue();

                //eventVenue.EventType = EventVenue.EventType.Remote;
                eventVenue.Type = (EventVenue.EventType)eventTypeVal;
                eventVenue.Address = Address;
                eventVenue.City = City;
                eventVenue.State = State;
                eventVenue.Country = Country;
                eventVenue.Zip = Zip;

                if (ModelState.IsValid)
                {
                    _context.Add(eventVenue);
                    _context.SaveChanges();
                    //_context.SaveChangesAsync();

                    // Get the last id from the database and return it
                    return eventVenue.EventVenueId;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return 0;
        }



        // GET: EventVenues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EventVenues == null)
            {
                return NotFound();
            }

            var eventVenue = await _context.EventVenues.FindAsync(id);
            if (eventVenue == null)
            {
                return NotFound();
            }
            return View(eventVenue);
        }

        // POST: EventVenues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventVenueId,Country,City,State,Address,Zip")] EventVenue eventVenue)
        {
            if (id != eventVenue.EventVenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventVenue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventVenueExists(eventVenue.EventVenueId))
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
            return View(eventVenue);
        }

        // GET: EventVenues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EventVenues == null)
            {
                return NotFound();
            }

            var eventVenue = await _context.EventVenues
                .FirstOrDefaultAsync(m => m.EventVenueId == id);
            if (eventVenue == null)
            {
                return NotFound();
            }

            return View(eventVenue);
        }

        // POST: EventVenues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EventVenues == null)
            {
                return Problem("Entity set 'ApplicationDbContext.EventVenues'  is null.");
            }
            var eventVenue = await _context.EventVenues.FindAsync(id);
            if (eventVenue != null)
            {
                _context.EventVenues.Remove(eventVenue);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventVenueExists(int id)
        {
          return (_context.EventVenues?.Any(e => e.EventVenueId == id)).GetValueOrDefault();
        }
    }
}
