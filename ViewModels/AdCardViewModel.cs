

using System.ComponentModel.DataAnnotations;
using RoomFinder4You.Models;

namespace RoomFinder4You.ViewModels
{
    public class AdCardViewModel{
    public int Id{get;set;}
    public String Title{get;set;}
    public String Description{get;set;} 
    public Byte[]? MainPhoto{get;set;}
    public String? PhotoFormat{get;set;}
    public String City{get;set;}
    public String Place{get;set;}
    }
}