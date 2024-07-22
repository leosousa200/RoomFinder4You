using System.ComponentModel.DataAnnotations;
using RoomFinder4You.Models;

namespace RoomFinder4You.ViewModels
{
    public class HomePageViewModel{
        public ICollection<AdCardViewModel> MorePopular;
        public String cityOneName;
        public ICollection<AdCardViewModel> CityOne;
        public String cityTwoName;
        public ICollection<AdCardViewModel> CityTwo;
    }
}