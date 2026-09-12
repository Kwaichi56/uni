using SportNutritionShop.Models;

namespace SportNutritionShop.Services
{
    public static class AuthService
    {
        public static bool TryLogin(string login, string password, out UserRole role)
        {
            return DatabaseService.TryLogin(login, password, out role);
        }
    }
}
