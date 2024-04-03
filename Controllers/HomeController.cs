using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomFinder4You.Data;
using RoomFinder4You.Models;
using RoomFinder4You.ViewModels;

namespace RoomFinder4You.Controllers;

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
            var applicationDbContext = _context.Ads.Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room)
                .OrderByDescending(a => a.ViewNumber)
                .Take(3).ToList();

            ICollection<AdCardViewModel> viewModel = new List<AdCardViewModel>();

            foreach(var ad in applicationDbContext){
                AdCardViewModel tempModel = new AdCardViewModel{
                    Id = ad.Id,
                    Title = ad.Title,
                    Description = ad.Description,
                    MainPhoto = ad.MainPhoto,
                    PhotoFormat = ad.PhotoFormat,
                    ViewNumber = ad.ViewNumber
                };
                viewModel.Add(tempModel);
            }
            return View(viewModel);
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
