using System.ComponentModel.DataAnnotations;

namespace RoomFinder4You.Models;

public class Feature{
    public int Id{get;set;}
    [Display(Name = "Valor")]
    public String Value{get;set;}
    [Display(Name = "Tipo")]
    public FeatureType featureType{get;set;}
    public int FeatureTypeId{get;set;}
}