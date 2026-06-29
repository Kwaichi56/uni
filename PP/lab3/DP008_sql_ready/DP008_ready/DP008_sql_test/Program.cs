using DP008_ICelebrity;
using DP008_sql;

namespace DP008_sql_test;

internal class Program
{
    static void Main(string[] args)
    {
        Repository.DatabaseFileName = "celebrities.db";

        string dbDirectory = Path.Combine(Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName, "Database");
        string dbFile = Path.Combine(dbDirectory, Repository.DatabaseFileName);
        if (File.Exists(dbFile))
            File.Delete(dbFile);

        using (ICelebrity<Celebrity> repo = Repository.Create("Database"))
        {
            Console.WriteLine("GetAllCelebrities");
            Print(repo.GetAllCelebrities());

            Console.WriteLine("AddCelebrity");
            repo.AddCelebrity(new Celebrity { Firstname = "Marlin", Surname = "Minsky", PhotoPath = "/Photo/Minsky.jpg" });
            Print(repo.GetAllCelebrities());

            Console.WriteLine("GetCelebrityIdByName, UpdCelebrity");
            int id = repo.GetCelebrityIdByName("Marlin");
            if (id >= 0)
                repo.UpdCelebrity(id, new Celebrity { Firstname = "Marvin", Surname = "Minsky", PhotoPath = "/Photo/Minsky.jpg" });
            Print(repo.GetAllCelebrities());

            Console.WriteLine("GetCelebrityIdByName, DelCelebrity");
            id = repo.GetCelebrityIdByName("Minsky");
            if (id >= 0)
                repo.DelCelebrity(id);
            Print(repo.GetAllCelebrities());

            Console.WriteLine("AddCelebrityAndGetId");
            id = repo.AddCelebrityAndGetId(new Celebrity { Firstname = "Marvin", Surname = "Minsky", PhotoPath = "/Photo/Minsky.jpg" });
            Console.WriteLine("Minsky Id = {0}", id);
            Print(repo.GetAllCelebrities());

            Console.WriteLine("SaveChanges = {0}", repo.SaveChanges());
        }

        using (ICelebrity<Celebrity> repo = Repository.Create("Database"))
        {
            Print(repo.GetAllCelebrities());
        }
    }

    private static void Print(List<Celebrity> celebrities)
    {
        celebrities.ForEach(c =>
            Console.WriteLine("Id = {0}, Firstname = {1}, Surname = {2}, PhotoPath = {3}",
                c.Id, c.Firstname, c.Surname, c.PhotoPath));
    }
}
