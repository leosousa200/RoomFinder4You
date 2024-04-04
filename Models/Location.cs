using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class Location
{
    public int Id { get; set; }
    [Display(Name = "Cidade")]
    public String City { get; set; }
    [Display(Name = "Local")]
    public String Place { get; set; }
}