using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public static class UploadHelper
{
    private const string uploadsFolder = "Uploads";
    private const string adsFolder = "Ads";
    private const string uploadFormats = ".png";
    public static string GetUploadFolder() { return uploadsFolder; }
    public static string GetAdsFolder() { return adsFolder; }
    public static string GetUploadFormats() { return uploadFormats; }
}