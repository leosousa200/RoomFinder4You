using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class Country{
    public int Id{get;set;}
    [Display(Name = "Nome")]
    public String Name{get;set;}
    public ICollection<City> Cities{get;set;}
    [Display(Name = "Número de anúncios")]
    public int NumberOfAds{get;set;} = 0;
}