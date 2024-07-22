using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class FeatureType{
    public int Id{get;set;}
    [Display(Name = "Tipo")]
    public String Name{get;set;}
    [Display(Name = "Sigla")]
    [StringLength(3,MinimumLength = 3)]
    public String Initials{get;set;}
    [Display(Name = "Mandatório")]
    public bool IsMandatory{get;set;}

}