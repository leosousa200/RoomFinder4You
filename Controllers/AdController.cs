using System.Security.Claims;
using Humanizer;
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
    /// <summary>
    /// Ad Controller.
    /// </summary>
    public class AdController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// [GET]
        /// Ad management page.
        /// </summary>
        /// <returns>View with list of ads.</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var ads = _context.Ads.
                Where(ad => ad.User == user)
                .Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room)
                .Include(a => a.room.location)
                .Include(a => a.room.location.city);
            ;
            return View(await ads.ToListAsync());
        }

        /// <summary>
        /// [GET]
        /// Ad details.
        /// </summary>
        /// <param name="id">Id of the ad.</param>
        /// <returns>View with the add, if possible.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ads == null) { return NotFound(); }

            var ad = await _context.Ads
                .Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room).ThenInclude(a => a.Features).ThenInclude(a => a.featureType)
                .Include(a => a.room.location)
                .Include(a => a.room.location.city)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ad == null) { return NotFound(); }

            // Increase the view number of the ad
            ad.ClickNumber++;

            // Prepare gallery images to be shown
            string galleryFolder = Path.Combine(_hostingEnvironment.ContentRootPath, UploadHelper.GetUploadFolder(),
             UploadHelper.GetAdsFolder(), "Ad_" + ad.Id);

            if (Directory.Exists(galleryFolder))
            {
                List<byte[]> imageData = ReadImageFilesFromDisk(new List<string>(Directory.GetFiles(galleryFolder, "")));
                ViewData["galleryImages"] = imageData;
            }
            ViewData["phoneNumber"] = ad.User.PhoneNumber;

            _context.Update(ad);
            await _context.SaveChangesAsync();

            return View(ad);
        }

        /// <summary>
        /// [GET]
        /// Ad create page.
        /// </summary>
        /// <returns>View with all the viewdata needed.</returns>
        public IActionResult Create()
        {
            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Status");
            ViewData["FeatureTypesMandatory"] = new SelectList(_context.FeatureTypes.Where(v => v.IsMandatory), "Initials", "Name");
            ViewData["FeatureTypesNonMandatory"] = new SelectList(_context.FeatureTypes.Where(v => !v.IsMandatory), "Initials", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");

            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Title,Description,AdStatusId,room.Price")] Ad ad, float price,
            [Bind("CBP,AQU,TAM,MOB,ELE,AGU,GAS")] FeaturesInitialsViewModel featuresInitials, IFormFile imagem,
            IFormFileCollection gallery, int cityId, string place)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            Ad? InitialAd = _context.Ads.Where(a => a.Id == ad.Id)
                .Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room).ThenInclude(a => a.Features).ThenInclude(a => a.featureType)
                .Include(a => a.room.location)
                .Include(a => a.room.location.city).FirstOrDefault();

            if (InitialAd == null)
            {
                return NotFound();
            }
            if (user != InitialAd.User)
            {
                return new NotEnoughPermissions();
            }


            if (InitialAd.Title != ad.Title)
                InitialAd.Title = ad.Title;

            if (InitialAd.Description != ad.Description)
                InitialAd.Description = ad.Description;

            if (InitialAd.AdStatusId != ad.AdStatusId)
                InitialAd.AdStatusId = ad.AdStatusId;

            if (InitialAd.room.location.city.Id != cityId)
                InitialAd.room.location.city = _context.Cities.Where(city => city.Id == cityId).FirstOrDefault()!;

            if (InitialAd.room.location.Place != place)
                InitialAd.room.location.Place = place;

            if (InitialAd.room.Price != price)
                InitialAd.room.Price = price;

            // Mandatory Features
            List<Feature>? features = FeaturesProcess(featuresInitials);
            if (features == null)
                return View(ad);
            if (InitialAd.room.Features != features)
                InitialAd.room.Features = features;

            // If Main Image changes
            byte[]? bytes;
            if ((bytes = ImageToByte(imagem)) != null)
            {
                InitialAd.PhotoFormat = imagem.ContentType;
                InitialAd.MainPhoto = null;
                InitialAd.MainPhoto = bytes;
            }

            if (gallery.Count != 0)
            {
                removeGallery(InitialAd.Id);
                UploadImages(InitialAd, gallery);
            }


            try
            {
                _context.Update(InitialAd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdExists(InitialAd.Id))
                {
                    return NotFound();
                }
            }


            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Status");
            ViewData["FeatureTypesMandatory"] = new SelectList(_context.FeatureTypes.Where(v => v.IsMandatory), "Initials", "Name");
            ViewData["FeatureTypesNonMandatory"] = new SelectList(_context.FeatureTypes.Where(v => !v.IsMandatory), "Initials", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            return View(ad);
        }














        /// <summary>
        /// [POST]
        /// Ad creation.
        /// </summary>
        /// <param name="ad">Data of the ad.</param>
        /// <param name="price">Price of the room.</param>
        /// <param name="featuresInitials">Initial features of the room.</param>
        /// <param name="imagem">Main image of the room.</param>
        /// <param name="gallery">Collection of images of the room.</param>
        /// <param name="cityId">Id of the city.</param>
        /// <param name="place">Name of the place.</param>
        /// <returns>Redirect to index.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,AdStatusId,room.Price")] Ad ad, float price,
            [Bind("CBP,AQU,TAM,MOB,ELE,AGU,GAS")] FeaturesInitialsViewModel featuresInitials, IFormFile imagem,
            IFormFileCollection gallery, int cityId, string place)
        {
            // Remove data not created yet
            ModelState.Remove(nameof(ad.room)); ModelState.Remove(nameof(ad.User));
            ModelState.Remove(nameof(ad.UserID)); ModelState.Remove(nameof(ad.adStatus));

            if (ModelState.IsValid)
            {
                // Room object creation
                Room room = new Room();
                room.Price = price;

                // Location bind to the room
                Location loc = new Location
                {
                    city = await _context.Cities.FindAsync(cityId),
                    Place = place
                };
                room.location = loc;
                ad.room = room;

                // Features extraction
                List<Feature>? features = FeaturesProcess(featuresInitials);
                if (features == null)
                    return View(ad);

                room.Features = features;

                // Main image logic
                byte[]? bytes;
                if ((bytes = ImageToByte(imagem)) != null)
                {
                    ad.PhotoFormat = imagem.ContentType;
                    ad.MainPhoto = bytes;
                }
                else { return View(ad); }

                // Link the user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ad.UserID = userId!;

                _context.Add(ad);

                // Increment ad number size in city and country:
                City? tempCity = await IncreaseCityAdNumber(cityId);
                if (tempCity == null) return View(ad);
                Country? tempCountry = await IncreaseCountryAdNumber(tempCity.CountryId);
                if (tempCountry == null) return View(ad);
                _context.Cities.Update(tempCity);
                _context.Countries.Update(tempCountry);

                // Save database changes
                await _context.SaveChangesAsync();

                // Save gallery'images
                UploadImages(ad, gallery);

                return RedirectToAction(nameof(Index));
            }

            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Status");
            ViewData["FeatureTypesMandatory"] = new SelectList(_context.FeatureTypes.Where(v => v.IsMandatory), "Initials", "Name");
            ViewData["FeatureTypesNonMandatory"] = new SelectList(_context.FeatureTypes.Where(v => !v.IsMandatory), "Initials", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");

            return View(ad);
        }

        /// <summary>
        /// [GET]
        /// Ad edit page.
        /// </summary>
        /// <param name="id">Id of the ad.</param>
        /// <returns>View with all the viewdata needed.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ads == null)
            {
                return NotFound();
            }

            var ad = await _context.Ads
                .Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room).ThenInclude(a => a.Features).ThenInclude(a => a.featureType)
                .Include(a => a.room.location)
                .Include(a => a.room.location.city)
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != ad.User)
            {
                return new NotEnoughPermissions();
            }

            if (ad == null)
            {
                return NotFound();
            }
            ViewData["AdStatusId"] = new SelectList(_context.AdsStatus, "Id", "Status");
            ViewData["FeatureTypesMandatory"] = new SelectList(_context.FeatureTypes.Where(v => v.IsMandatory), "Initials", "Name");
            ViewData["FeatureTypesNonMandatory"] = new SelectList(_context.FeatureTypes.Where(v => !v.IsMandatory), "Initials", "Name");
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            return View(ad);
        }

        /// <summary>
        /// [GET]
        /// Ad delete page.
        /// </summary>
        /// <param name="id">Id of the ad.</param>
        /// <returns>View of deletion with the data of the ad.</returns>
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

        /// <summary>
        /// [POST]
        /// Delete an ad.
        /// </summary>
        /// <param name="id">Id of the ad.</param>
        /// <returns>Redirect to index.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ads == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ads'  is null.");
            }
            var ad = _context.Ads.Include(ad => ad.room)
                .ThenInclude(room => room.Features)
                .Include(ad => ad.room)
                .ThenInclude(a => a.location)
                .ThenInclude(a => a.city)
                .ThenInclude(a => a.country).First(ad => ad.Id == id);
            if (ad != null)
            {

                //increase ad number size to city and country:
                City tempCity = ad.room.location.city;
                tempCity.NumberOfAds--;
                Country tempCountry = tempCity.country;
                tempCountry.NumberOfAds--;
                Location tempLocation = ad.room.location;
                _context.Cities.Update(tempCity);
                _context.Countries.Update(tempCountry);
                if (ad.room.Features != null)
                    _context.Features.RemoveRange(ad.room.Features);

                _context.Rooms.Remove(ad.room);
                _context.Locations.Remove(tempLocation);

                // remove image folder
                string uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, UploadHelper.GetUploadFolder());
                uploadsFolder = Path.Combine(uploadsFolder, UploadHelper.GetAdsFolder());

                uploadsFolder = Path.Combine(uploadsFolder, ("Ad_" + ad.Id));
                Directory.Delete(uploadsFolder, true);

                _context.Ads.Remove(ad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool removeGallery(int adId)
        {
            string uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, UploadHelper.GetUploadFolder());
            uploadsFolder = Path.Combine(uploadsFolder, UploadHelper.GetAdsFolder());

            uploadsFolder = Path.Combine(uploadsFolder, ("Ad_" + adId));
            Directory.Delete(uploadsFolder, true);
            return true;
        }

        /// <summary>
        /// Informs if the ad exists.
        /// </summary>
        /// <param name="id">Id of the ad to search.</param>
        /// <returns>True if exists.</returns>
        private bool AdExists(int id)
        {
            return (_context.Ads?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Extracts a list of features from
        /// a model.
        /// </summary>
        /// <param name="featuresInitials">Features information.</param>
        /// <returns>The list of features extracted.</returns>
        private List<Feature>? FeaturesProcess(FeaturesInitialsViewModel featuresInitials)
        {
            if (String.IsNullOrEmpty(featuresInitials.CBP) || String.IsNullOrEmpty(featuresInitials.AQU) || String.IsNullOrEmpty(featuresInitials.TAM) || String.IsNullOrEmpty(featuresInitials.MOB))
                return null;

            // Get mandatory features
            List<Feature> features = AppendMandatoryFeatures(featuresInitials);

            // Get non mandatory features
            features = AppendNonMandatoryFeatures(featuresInitials, features);

            return features;
        }

        /// <summary>
        /// Increase the number of the ads related
        /// to a city.
        /// </summary>
        /// <param name="cityId">Id of the city.</param>
        /// <returns>City obtained, with a increased number of ads.</returns>
        private async Task<City?> IncreaseCityAdNumber(int cityId)
        {
            City? city = await _context.Cities.Include(city => city.country).FirstOrDefaultAsync(city => city.Id == cityId);
            if (city == null) return null;

            city.NumberOfAds++;
            return city;
        }

        /// <summary>
        /// Increase the number of the ads related
        /// to a country.
        /// </summary>
        /// <param name="countryId">Id of the country.</param>
        /// <returns>Country obtained, with a increased number of ads.</returns>
        private async Task<Country?> IncreaseCountryAdNumber(int countryId)
        {
            Country? country = await _context.Countries.FirstOrDefaultAsync(country => country.Id == countryId);
            if (country == null) return null;

            country.NumberOfAds++;
            return country;
        }


        /// <summary>
        /// Extracts a list of features from
        /// a model.
        /// </summary>
        /// <param name="featuresInitials">Features information.</param>
        /// <returns>The list of features extracted.</returns>
        private Byte[]? ImageToByte(IFormFile image)
        {
            if (image == null)
                return null;

            // Transform image into bytes
            var memoryStream = new MemoryStream();
            image.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }


        /// <summary>
        /// Extracts a list of mandatory
        /// features from a model.
        /// </summary>
        /// <param name="featuresInitials">Features information.</param>
        /// <returns>The list of the mandatory features extracted.</returns>
        private List<Feature> AppendMandatoryFeatures(FeaturesInitialsViewModel featuresInitials)
        {
            List<Feature> features = new List<Feature>
            {
                new Feature
                {
                    featureType = _context.FeatureTypes.First(v => v.Initials.Equals("CBP")),
                    Value = featuresInitials.CBP
                },
                new Feature
                {
                    featureType = _context.FeatureTypes.First(v => v.Initials.Equals("AQU")),
                    Value = featuresInitials.AQU
                },
                new Feature
                {
                    featureType = _context.FeatureTypes.First(v => v.Initials.Equals("TAM")),
                    Value = featuresInitials.TAM
                },
                new Feature
                {
                    featureType = _context.FeatureTypes.First(v => v.Initials.Equals("MOB")),
                    Value = featuresInitials.MOB
                }
            };
            return features;
        }

        /// <summary>
        /// Extracts a list of non mandatory
        /// features from a model and added it
        /// to the received list.
        /// </summary>
        /// <param name="featuresInitials">Features information.</param>
        /// <param name="features">List with mandatory features.</param>
        /// <returns>The list of all features extracted.</returns>
        private List<Feature> AppendNonMandatoryFeatures(FeaturesInitialsViewModel featuresInitials, List<Feature> features)
        {

            if (!String.IsNullOrEmpty(featuresInitials.ELE))
            {
                features.Add(new Feature
                {
                    featureType = _context.FeatureTypes.First(v => v.Initials.Equals("ELE")),
                    Value = featuresInitials.ELE
                });
            }

            if (!String.IsNullOrEmpty(featuresInitials.AGU))
            {
                features.Add(new Feature
                {
                    featureType = _context.FeatureTypes.First(v => v.Initials.Equals("AGU")),
                    Value = featuresInitials.AGU
                });
            }

            if (!String.IsNullOrEmpty(featuresInitials.GAS))
            {
                features.Add(new Feature
                {
                    featureType = _context.FeatureTypes.First(v => v.Initials.Equals("GAS")),
                    Value = featuresInitials.GAS
                });
            }

            return features;
        }

        /// <summary>
        /// Upload images to the folder of 
        /// images of the ad.
        /// </summary>
        /// <param name="ad">Ad data.</param>
        /// <param name="formFiles">Collection of files.</param>
        /// <returns>True if all went well.</returns>
        private bool UploadImages(Ad ad, IFormFileCollection formFiles)
        {
            if (formFiles == null)
                return false;

            string uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, UploadHelper.GetUploadFolder());
            uploadsFolder = Path.Combine(uploadsFolder, UploadHelper.GetAdsFolder());

            // if upload folder dont exist
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);


            uploadsFolder = Path.Combine(uploadsFolder, ("Ad_" + ad.Id));

            // if upload folder for the add dont exist
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            int i = 1;
            foreach (var file in formFiles)
            {

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName) + ".png";
                string finalFilePath = uploadsFolder + "/img_" + i + UploadHelper.GetUploadFormats();
                using (var fileStream = new FileStream(finalFilePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                i++;
            }
            return true;
        }

        /// <summary>
        /// [IA function generated] 
        /// Read the images in the indicated paths
        /// and transforms it to bytes, so we dont need
        /// to indicate the URL in the view.
        /// </summary>
        /// <param name="filePaths">List of all paths</param>
        /// <returns>List of the bytes of the images.</returns>
        private List<byte[]> ReadImageFilesFromDisk(List<string> filePaths)
        {
            var imageDatas = new List<byte[]>();

            foreach (var filePath in filePaths)
            {
                byte[] imageData = System.IO.File.ReadAllBytes(filePath);
                imageDatas.Add(imageData);
            }

            return imageDatas;
        }
    }
}
