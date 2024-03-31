

using System.ComponentModel.DataAnnotations;
using RoomFinder4You.Models;

namespace RoomFinder4You.ViewModels
{
    public class AdCreateViewModel{
    public String Title{get;set;}
    public String Description{get;set;} 
    public AdStatus adStatus{get;set;}
    public int AdStatusId{get;set;}
    public byte[]? MainPhoto;
    public string? PhotoFormat;
    public String UserID {get;set;}
    public ApplicationUser User{get;set;}
    public ICollection<Feature> Features{get;set;}

    }
}