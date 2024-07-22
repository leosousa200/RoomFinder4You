namespace RoomFinder4You.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser: IdentityUser {
    public string FirstName{get;set;}
    public string LastName{get;set;}
    public ICollection<Ad> Ads{get;set;}
}