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
using System.Data;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;

namespace COMP2084_Project_Eventour.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
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


        [AllowAnonymous]
        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
               .Include(item => item.EventDetail)
               .ThenInclude(item => item.EventVenue)
               .Include(item => item.EventDetail)
               .ThenInclude(item => item.Category)
               .Include(item => item.User)
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
            "EventId,Title, Description,EventDetailId, UserId, EventDetail.Photo"
            )] Event @event, IFormFile? Photo)
        {

            if (ModelState.IsValid)
            {
                try
                {
                
                    // Create the Venue into the database and return the ID.
                    int eventType = int.Parse(HttpContext.Request.Form["EventDetail.EventVenue.Type"]);
                    string Address = HttpContext.Request.Form["EventDetail.EventVenue.Address"];
                    string City = HttpContext.Request.Form["EventDetail.EventVenue.City"];
                    string State = HttpContext.Request.Form["EventDetail.EventVenue.State"];
                    string Country = HttpContext.Request.Form["EventDetail.EventVenue.Country"];
                    string Zip = HttpContext.Request.Form["EventDetail.EventVenue.Zip"];

                    // need to get event detail id first then add to event.
                    var EventVenueId = new EventVenuesController(_context).CreateVenue(eventType,Address, City, State, Country, Zip);

                    // Create the Details into the database and return the ID.
                    double Price = Double.Parse(HttpContext.Request.Form["EventDetail.Price"]);
                    DateTime StartDate = DateTime.Parse(HttpContext.Request.Form["EventDetail.StartDate"]);
                    DateTime EndDate = DateTime.Parse(HttpContext.Request.Form["EventDetail.EndDate"]);

                
                    int CategoryId = int.Parse(HttpContext.Request.Form["EventDetail.CategoryId"]);

                    // Builds the event details and returns it ID.
                    var EventDetailId = new EventDetailsController(_context).CreateDetail(Price, StartDate, EndDate, CategoryId, EventVenueId);

                    // Need to get the photo and save it.
                    string? PhotoName = "placeholder.png";

                    if (Photo != null)
                    {
                        var fileName = UploadPhoto(Photo);
                        PhotoName = fileName;
                    }

                    @event.EventDetailId = EventDetailId;
                    @event.UserId = 1;
                    @event.createdOn = DateTime.Now;
                    @event.Photo = PhotoName;

                    _context.Add(@event);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
            @event.EventDetail = await _context.EventDetails.FindAsync(@event.EventDetailId);
            @event.EventDetail.EventVenue = await _context.EventVenues.FindAsync(@event.EventDetail.EventVenueId);

            if (@event == null)
            {
                return NotFound();
            }


            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");


            //ViewData["EventDetailId"] = new SelectList(_context.EventDetails, "EventDetailId", "EventDetailId", @event.EventDetailId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", @event.UserId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,EventDetailId,UserId")] Event @event, IFormFile? Photo, string? CurrentPhoto)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create instance of the object.
                    EventDetail eventDetail = new EventDetail();
                    EventVenue eventVenue = new EventVenue();

                    //Get the current state from the database.
                    eventDetail = _context.EventDetails.Find(@event.EventDetailId);
                    eventVenue = _context.EventVenues.Find(eventDetail.EventVenueId);
                      

                    // Get the updated form info.
                    int eventType = int.Parse(HttpContext.Request.Form["EventDetail.EventVenue.Type"]);
                    string Address = HttpContext.Request.Form["EventDetail.EventVenue.Address"];
                    string City = HttpContext.Request.Form["EventDetail.EventVenue.City"];
                    string State = HttpContext.Request.Form["EventDetail.EventVenue.State"];
                    string Country = HttpContext.Request.Form["EventDetail.EventVenue.Country"];
                    string Zip = HttpContext.Request.Form["EventDetail.EventVenue.Zip"];

                    // Create the Details into the database and return the ID.
                    double Price = Double.Parse(HttpContext.Request.Form["EventDetail.Price"]);
                    DateTime StartDate = DateTime.Parse(HttpContext.Request.Form["EventDetail.StartDate"]);
                    DateTime EndDate = DateTime.Parse(HttpContext.Request.Form["EventDetail.EndDate"]);

                    int CategoryId = int.Parse(HttpContext.Request.Form["EventDetail.CategoryId"]);


                    // Update the model variables.
                    eventVenue.Type = (EventVenue.EventType)eventType;
                    eventVenue.Address = Address;
                    eventVenue.City = City;
                    eventVenue.State = State;
                    eventVenue.Country = Country;
                    eventVenue.Zip = Zip;

                    eventDetail.CategoryId = CategoryId;
                    eventDetail.Price = Price;
                    eventDetail.StartDate = StartDate;
                    eventDetail.EndDate = EndDate;


                    // Update the model details
                    _context.EventVenues.Update(eventVenue);
                    _context.EventDetails.Update(eventDetail);


                    // Handles replacing the photo or keeping it.
                    if (Photo != null)
                    {
                        var fileName = UploadPhoto(Photo);
                        @event.Photo = fileName;
                    }
                    else
                    {
                        @event.Photo = CurrentPhoto;
                    }
    
                   
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

            var @event = await _context.Events
                .Include(item => item.EventDetail)
                .ThenInclude(item => item.EventVenue)
                .Include(item => item.EventDetail)
                .ThenInclude(item => item.Category)
                .Include(item => item.User)
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

                // Gets the object by the ID for each instance in the database
                EventDetail eventDetail = await _context.EventDetails.FindAsync(@event.EventDetailId);
                EventVenue eventVenue = await _context.EventVenues.FindAsync(@event.EventDetail.EventVenueId);


                // Removes them in a specific order so that the foreign keys doesn't throw exception error.
                _context.Events.Remove(@event);
                _context.EventDetails.Remove(eventDetail);
                _context.EventVenues.Remove(eventVenue);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
          return (_context.Events?.Any(e => e.EventId == id)).GetValueOrDefault();
        }

        /**
         * Code taken from class lecture..
         */
        private static string UploadPhoto(IFormFile Photo)
        {
            //get temp location of the uploaded file
            var filePath = Path.GetTempFileName();

            //create unique name to prevent overwrites
            var fileName = Guid.NewGuid() + "-" + Photo.FileName;

            // set destination path to wwwroot/assets/images/events/
            var uploadPath = System.IO.Directory.GetCurrentDirectory() + "//wwwroot//assets//images//events//" + fileName;

            // copy the file to the target dir
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                Photo.CopyTo(stream);
            }

            return fileName;
        }

    }
}
