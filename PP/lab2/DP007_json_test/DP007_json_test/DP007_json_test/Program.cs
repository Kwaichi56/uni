using System;
using System.Collections.Generic;
using DP007_ICelebrity;
using DP007_json;

namespace DP007_json_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Repository.JSONFileName = "celebrities.json";

            using (ICelebrity<Celebrity> repo = Repository.Create("Celebrities"))  // создаем экземпляр класса Repository
            {

                Console.WriteLine("GetAllCelebrities"); 
                List<Celebrity> celebrities = repo.GetAllCelebrities(); 
                celebrities.ForEach(c => 
                    Console.WriteLine("Id = {0}, Firstname = {1}, Surname = {2}, PhotoPath = {3}", 
                        c.Id, c.Firstname, c.Surname, c.PhotoPath)); 




                Console.WriteLine("AddCelebrity");
                repo.AddCelebrity(new Celebrity(0, "Marlin", "Minsky", "/Photo/Minsky.jpg")); // добавляем новую знаменитость
                celebrities = repo.GetAllCelebrities(); 
                celebrities.ForEach(c =>
                    Console.WriteLine("Id = {0}, Firstname = {1}, Surname = {2}, PhotoPath = {3}",
                        c.Id, c.Firstname, c.Surname, c.PhotoPath)); 




                Console.WriteLine("GetCelebrityIdByName, UpdCelebrity");
                int id = repo.GetCelebrityIdByName("Marlin"); // получаем ID знаменитости по имени
                if (id >= 0)
                    repo.UpdCelebrity(id, new Celebrity(0, "Marvin", "Minsky", "/Photo/Minsky.jpg")); 
                celebrities = repo.GetAllCelebrities(); 
                celebrities.ForEach(c =>
                    Console.WriteLine("Id = {0}, Firstname = {1}, Surname = {2}, PhotoPath = {3}",
                        c.Id, c.Firstname, c.Surname, c.PhotoPath)); 




                Console.WriteLine("GetCelebrityIdByName, DelCelebrity");
                id = repo.GetCelebrityIdByName("Minsky"); // получаем ID знаменитости по имени
                if (id >= 0)
                    repo.DelCelebrity(id); 
                celebrities = repo.GetAllCelebrities();
                celebrities.ForEach(c =>
                    Console.WriteLine("Id = {0}, Firstname = {1}, Surname = {2}, PhotoPath = {3}",
                        c.Id, c.Firstname, c.Surname, c.PhotoPath)); 




                Console.WriteLine("AddCelebrityAndGetId");
                id = repo.AddCelebrityAndGetId(new Celebrity(0, "Marvin", "Minsky", "/Photo/Minsky.jpg")); // добавляем новую знаменитость и получаем её ID
                Console.WriteLine("Minsky Id = {0}", id);
                celebrities = repo.GetAllCelebrities(); 
                celebrities.ForEach(c =>
                    Console.WriteLine("Id = {0}, Firstname = {1}, Surname = {2}, PhotoPath = {3}",
                        c.Id, c.Firstname, c.Surname, c.PhotoPath));




                Console.WriteLine("SaveChanges = {0}", repo.SaveChanges()); // сохраняем изменения
                
            }

            using (ICelebrity<Celebrity> repo = Repository.Create("Celebrities")) // создаем экземпляр класса Repository
            {
                List<Celebrity> celebrities = repo.GetAllCelebrities(); 
                celebrities.ForEach(c =>
                    Console.WriteLine("Id = {0}, Firstname = {1}, Surname = {2}, PhotoPath = {3}",
                        c.Id, c.Firstname, c.Surname, c.PhotoPath));
            }
        }
    }
}