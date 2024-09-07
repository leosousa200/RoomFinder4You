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
            .OrderByDescending(a => a.ClickNumber)
            .Take(3).ToList();

        var cityOneData = applicationDbContext
            .Where(a => a.room.location.city.NumberOfAds >= 3)
            .ToList();
        cityOneData = cityOneData
            .OrderBy(r => Guid.NewGuid())
            .Take(3).ToList();

        string cityOneName = "";
        if (morePopulardata.Count > 0)
        {
            morePopulardata.ToList().ForEach(a => a.ViewNumber++);
            _context.Ads.UpdateRange(morePopulardata);
        }

        if (cityOneData.Count > 0)
        {
            cityOneName = cityOneData.First().room.location.city.Name;
            cityOneData.ToList().ForEach(a => a.ViewNumber++);
            _context.Ads.UpdateRange(cityOneData);
        }
        var cityTwoData = applicationDbContext
            .Where(a => a.room.location.city.NumberOfAds >= 3 && !a.room.location.city.Name.Equals(cityOneName))
            .ToList();

        cityTwoData = cityTwoData.OrderBy(r => Guid.NewGuid())
            .Take(3).ToList();

        string cityTwoName = "";
        if (cityTwoData.Count > 0)
        {
            cityTwoName = cityTwoData.First().room.location.city.Name;
            cityTwoData.ToList().ForEach(a => a.ViewNumber++);
        }

        ICollection<AdCardViewModel> morePopular = setCardInfo(morePopulardata);
        ICollection<AdCardViewModel> cityOne = setCardInfo(cityOneData);
        ICollection<AdCardViewModel> cityTwo = setCardInfo(cityTwoData);

        HomePageViewModel homePageViewModel = new HomePageViewModel
        {
            MorePopular = morePopular,
            CityOne = cityOne,
            cityOneName = cityOneName,
            CityTwo = cityTwo,
            cityTwoName = cityTwoName
        };

        await _context.SaveChangesAsync();
        return View(homePageViewModel);
    }

    private ICollection<AdCardViewModel> setCardInfo(List<Ad> ads)
    {
        ICollection<AdCardViewModel> cards = new List<AdCardViewModel>();
        foreach (var ad in ads)
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
            cards.Add(tempModel);
        }
        return cards;
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
