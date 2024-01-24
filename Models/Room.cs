using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class Room{
    public int Id{get;set;}
    [Display(Name = "Preço")]
    public float Price{get;set;}
    [Display(Name = "Características")]
    public ICollection<Feature> Features{get;set;}
}