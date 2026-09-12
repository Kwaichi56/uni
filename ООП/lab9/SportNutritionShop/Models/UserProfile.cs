using SportNutritionShop.Helpers;

namespace SportNutritionShop.Models
{
    public class UserProfile : BaseViewModel
    {
        private string _firstName = "";
        private string _lastName = "";
        private string _email = "";
        private string _phone = "";

        public string Login { get; set; } = "";
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }
    }
}
