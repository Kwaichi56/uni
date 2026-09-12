using SportNutritionShop.Models;

namespace SportNutritionShop.Services;

public static class ProfileService
{
    public static UserProfile LoadProfile(string login) => DatabaseService.LoadProfile(login);

    public static void SaveProfile(UserProfile profile) => DatabaseService.SaveProfile(profile);
}
