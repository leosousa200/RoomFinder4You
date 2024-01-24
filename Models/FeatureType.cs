using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class FeatureType{
    public int Id{get;set;}
    [Display(Name = "Tipo")]
    public String Name{get;set;}
}