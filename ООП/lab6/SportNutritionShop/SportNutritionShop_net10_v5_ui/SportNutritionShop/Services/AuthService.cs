using SportNutritionShop.Models;

namespace SportNutritionShop.Services
{
    public static class AuthService
    {
        public static bool TryLogin(string login, string password, out UserRole role)
        {
            role = UserRole.Client;
            if (login == "admin" && password == "123")
            {
                role = UserRole.Admin;
                return true;
            }
            if (login == "client" && password == "123")
            {
                role = UserRole.Client;
                return true;
            }
            return false;
        }
    }
}
