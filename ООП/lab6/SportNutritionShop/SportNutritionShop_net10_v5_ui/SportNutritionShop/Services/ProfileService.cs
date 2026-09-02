using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SportNutritionShop.Models;

namespace SportNutritionShop.Services
{
    public static class ProfileService
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "profiles.json");

        public static UserProfile LoadProfile(string login)
        {
            var all = LoadAll();
            if (all.TryGetValue(login, out var profile))
                return profile;

            var created = new UserProfile { Login = login, FirstName = login, LastName = "", Email = "", Phone = "" };
            all[login] = created;
            SaveAll(all);
            return created;
        }

        public static void SaveProfile(UserProfile profile)
        {
            var all = LoadAll();
            all[profile.Login] = profile;
            SaveAll(all);
        }

        private static Dictionary<string, UserProfile> LoadAll()
        {
            try
            {
                if (!File.Exists(FilePath)) return new Dictionary<string, UserProfile>();
                var json = File.ReadAllText(FilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, UserProfile>>(json);
                return data ?? new Dictionary<string, UserProfile>();
            }
            catch
            {
                return new Dictionary<string, UserProfile>();
            }
        }

        private static void SaveAll(Dictionary<string, UserProfile> all)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(FilePath, JsonSerializer.Serialize(all, options));
        }
    }
}
