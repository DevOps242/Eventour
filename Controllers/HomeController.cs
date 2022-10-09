using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using COMP2084_Project_Eventour.Models;
using Microsoft.EntityFrameworkCore;
using COMP2084_Project_Eventour.Data;

namespace COMP2084_Project_Eventour.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
       
        var applicationDbContext = _context.Events
                .Include(x => x.User)
                .Include(x => x.EventDetail)
                .ThenInclude(x => x.Category)
                .Include(x => x.EventDetail)
                .ThenInclude(x => x.EventVenue);


        return View(await applicationDbContext.ToListAsync());
        //return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

