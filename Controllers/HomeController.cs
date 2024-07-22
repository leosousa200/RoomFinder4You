using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
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
            .Include(a => a.room.location)
            .ThenInclude(a => a.city);

        var morePopulardata = applicationDbContext
            .OrderByDescending(a => a.ViewNumber)
            .Take(3).ToList();

        var cityOneData = applicationDbContext
            .Where(a => a.room.location.city.NumberOfAds >= 3)
            .ToList();
        cityOneData = cityOneData
            .OrderBy(r => Guid.NewGuid())
            .Take(3).ToList();

        string cityOneName = "";
        if (cityOneData.Count > 0)
            cityOneName = cityOneData.First().room.location.city.Name;

        var cityTwoData = applicationDbContext
            .Where(a => a.room.location.city.NumberOfAds >= 3 && !a.room.location.city.Name.Equals(cityOneName))
            .ToList();

        cityTwoData = cityTwoData.OrderBy(r => Guid.NewGuid())
            .Take(3).ToList();

        string cityTwoName = "";
        if (cityTwoData.Count > 0)
            cityTwoName = cityTwoData.First().room.location.city.Name;


        ICollection<AdCardViewModel> morePopular = new List<AdCardViewModel>();
        ICollection<AdCardViewModel> cityOne = new List<AdCardViewModel>();
        ICollection<AdCardViewModel> cityTwo = new List<AdCardViewModel>();

        foreach (var ad in morePopulardata)
        {
            AdCardViewModel tempModel = new AdCardViewModel
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                MainPhoto = ad.MainPhoto,
                PhotoFormat = ad.PhotoFormat,
                City = ad.room.location.city.Name,
                Place = ad.room.location.Place,
                Price = ad.room.Price
            };
            morePopular.Add(tempModel);
        }

        foreach (var ad in cityOneData)
        {
            AdCardViewModel tempModel = new AdCardViewModel
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                MainPhoto = ad.MainPhoto,
                PhotoFormat = ad.PhotoFormat,
                City = ad.room.location.city.Name,
                Place = ad.room.location.Place,
                Price = ad.room.Price

            };
            cityOne.Add(tempModel);
        }

        foreach (var ad in cityTwoData)
        {
            AdCardViewModel tempModel = new AdCardViewModel
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                MainPhoto = ad.MainPhoto,
                PhotoFormat = ad.PhotoFormat,
                City = ad.room.location.city.Name,
                Place = ad.room.location.Place,
                Price = ad.room.Price

            };
            cityTwo.Add(tempModel);
        }


        HomePageViewModel homePageViewModel = new HomePageViewModel
        {
            MorePopular = morePopular,
            CityOne = cityOne,
            cityOneName = cityOneName,
            CityTwo = cityTwo,
            cityTwoName = cityTwoName
        };

        return View(homePageViewModel);
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
