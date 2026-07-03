using DP007_ICelebrity;
using DP007_ICelebrity.lib;
using DP007_json;
using DP007_json.lib;
using System;
using System.Collections.Generic;

namespace DP007_json_test
{
    class Program
    {
        static void Main(string[] args)
        {

            Repository.JSONFileName = "celebrities.json";

            using (ICelebrity<Celebrity> repo = Repository.Create("Celebrities"))
            {
                Console.WriteLine("--- GetAllCelebrities ---");
                List<Celebrity> celebrities = repo.GetAllCelebrities();
                celebrities.ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath}"));

                Console.WriteLine("\n--- AddCelebrity ---");
                repo.AddCelebrity(new Celebrity(0, "Marlin", "Minsky", "/Photo/Minsky.jpg"));
                repo.GetAllCelebrities().ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath}"));

                Console.WriteLine("\n--- GetCelebrityIdByName, UpdCelebrity ---");
                int id = repo.GetCelebrityIdByName("Marlin");
                if (id >= 0)
                    repo.UpdCelebrity(id, new Celebrity(id, "Marvin", "Minsky", "/Photo/Minsky.jpg"));

                repo.GetAllCelebrities().ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath}"));

                Console.WriteLine("\n--- GetCelebrityIdByName, DelCelebrity ---");
                id = repo.GetCelebrityIdByName("Minsky");
                if (id >= 0)
                    repo.DelCelebrity(id);

                repo.GetAllCelebrities().ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath}"));

                Console.WriteLine("\n--- AddCelebrityAndGetId ---");
                int newId = repo.AddCelebrityAndGetId(new Celebrity(0, "Marvin", "Minsky", "/Photo/Minsky.jpg"));
                Console.WriteLine($"Minsky Id = {newId}");

                repo.GetAllCelebrities().ForEach(c => Console.WriteLine($"Id = {c.Id}, Firstname = {c.Firstname}, Surname = {c.Surname}, PhotoPath = {c.PhotoPath}"));

                Console.WriteLine($"\nSaveChanges = {repo.SaveChanges()}");
            }
        }
    }
}