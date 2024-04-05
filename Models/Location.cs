using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class Location
{
    public int Id { get; set; }
    [Display(Name = "Cidade")]
    public City city { get; set; }
    [Display(Name = "Local")]
    public String Place { get; set; }
}