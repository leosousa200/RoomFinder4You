using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class AdStatus{
    public int Id{get;set;}
    [Display(Name = "Estado")]
    public String Status{get;set;}
}