using System.Security.Claims;
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
            var ads = _context.Ads.
                Include(a => a.User)
                .Include(a => a.adStatus)
                .Include(a => a.room);
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
                .Include(a => a.room)
                .ThenInclude(a => a.Features)
                .ThenInclude(a => a.featureType)
                .Include(a => a.room.location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ad == null) { return NotFound(); }

            // Increase the view number of the ad
            ad.ViewNumber++;

            // Prepare gallery images to be shown
            string galleryFolder = Path.Combine(_hostingEnvironment.ContentRootPath, UploadHelper.GetUploadFolder(),
             UploadHelper.GetAdsFolder(), "Ad_" + ad.Id);

            if (Directory.Exists(galleryFolder))
            {
                List<String> images = new List<string>(Directory.GetFiles(galleryFolder, ""));
                List<byte[]> imageData = ReadImageFilesFromDisk(images);
                ViewData["galleryImages"] = imageData;
            }

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
                // Location logic
                Location loc = new Location
                {
                    city = await _context.Cities.FindAsync(cityId),
                    Place = place
                };
                room.location = loc;
                ad.room = room;


                //Features logic
                if (String.IsNullOrEmpty(featuresInitials.CBP) || String.IsNullOrEmpty(featuresInitials.AQU) || String.IsNullOrEmpty(featuresInitials.TAM) || String.IsNullOrEmpty(featuresInitials.MOB))
                    return View(ad);


                Feature featureCBP = new Feature();
                featureCBP.featureType = _context.FeatureTypes.First(v => v.Initials.Equals("CBP"));
                featureCBP.Value = featuresInitials.CBP;
                room.Features.Add(featureCBP);

                Feature featureAQU = new Feature();
                featureAQU.featureType = _context.FeatureTypes.First(v => v.Initials.Equals("AQU"));
                featureAQU.Value = featuresInitials.AQU;
                room.Features.Add(featureAQU);

                Feature featureTAM = new Feature();
                featureTAM.featureType = _context.FeatureTypes.First(v => v.Initials.Equals("TAM"));
                featureTAM.Value = featuresInitials.TAM;
                room.Features.Add(featureTAM);

                Feature featureMOB = new Feature();
                featureMOB.featureType = _context.FeatureTypes.First(v => v.Initials.Equals("MOB"));
                featureMOB.Value = featuresInitials.MOB;
                room.Features.Add(featureMOB);

                // non mandatory features ELE,AGU,GAS
                if (!String.IsNullOrEmpty(featuresInitials.ELE))
                {
                    Feature featureELE = new Feature();
                    featureELE.featureType = _context.FeatureTypes.First(v => v.Initials.Equals("ELE"));
                    featureELE.Value = featuresInitials.ELE;
                    room.Features.Add(featureELE);
                }

                if (!String.IsNullOrEmpty(featuresInitials.AGU))
                {
                    Feature featureAGU = new Feature();
                    featureAGU.featureType = _context.FeatureTypes.First(v => v.Initials.Equals("AGU"));
                    featureAGU.Value = featuresInitials.AGU;
                    room.Features.Add(featureAGU);
                }

                if (!String.IsNullOrEmpty(featuresInitials.GAS))
                {
                    Feature featureGAS = new Feature();
                    featureGAS.featureType = _context.FeatureTypes.First(v => v.Initials.Equals("GAS"));
                    featureGAS.Value = featuresInitials.GAS;
                    room.Features.Add(featureGAS);
                }

                //image handling

                if (imagem != null)
                {

                    ad.PhotoFormat = imagem.ContentType;
                    var memoryStream = new MemoryStream();
                    imagem.CopyTo(memoryStream);
                    ad.MainPhoto = memoryStream.ToArray();

                }
                else
                {
                }

                // link user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ad.UserID = userId;


                // end
                _context.Add(ad);

                //increase ad number size to city and country:
                City tempCity = await _context.Cities.Include(city => city.country).FirstOrDefaultAsync(city => city.Id == cityId);
                tempCity.NumberOfAds++;
                Country tempCountry = tempCity.country;
                tempCountry.NumberOfAds++;
                _context.Cities.Update(tempCity);
                _context.Countries.Update(tempCountry);

                await _context.SaveChangesAsync();
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

        /// <summary>
        /// [Post]
        /// Edit an ad.
        /// </summary>
        /// <param name="id">Id of the ad.</param>
        /// <param name="ad">Data of the ad.</param>
        /// <returns>Redirect to index.</returns>
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
                _context.Cities.Update(tempCity);
                _context.Countries.Update(tempCountry);
                if (ad.room.Features != null)
                    _context.Features.RemoveRange(ad.room.Features);

                _context.Rooms.Remove(ad.room);
                _context.Ads.Remove(ad);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
