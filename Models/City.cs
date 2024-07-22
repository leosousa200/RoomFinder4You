using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class City{
    public int Id{get;set;}
    [Display(Name = "Cidade")]
    public String Name{get;set;}
    [Display(Name = "Número de anúncios")]
    public int NumberOfAds{get;set;} = 0;
    [Display(Name = "País")]
    public Country country { get; set; }
    public int CountryId { get; set; }

}