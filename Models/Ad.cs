using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class Ad{
    public int Id{get;set;}
    [Display(Name = "Título")]
    public String Title{get;set;}
    [Display(Name = "Descrição")]
    public String Description{get;set;} 
    [Display(Name = "Quarto")]
    public Room room{get;set;}
    public int RoomId{get;set;}
    [Display(Name = "Estado")]
    public AdStatus adStatus{get;set;}
    public int AdStatusId{get;set;}
    public byte[]? MainPhoto;
    public string? PhotoFormat;
    public String UserID {get;set;}
    public ApplicationUser User{get;set;}
}