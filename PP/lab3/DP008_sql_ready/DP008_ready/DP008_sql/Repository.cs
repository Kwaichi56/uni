using DP008_ICelebrity;
using Microsoft.EntityFrameworkCore;

namespace DP008_sql;

public sealed class Repository : ICelebrity<Celebrity>
{
    public static string DatabaseFileName = "celebrities.db";
    public static string BasePath { get; private set; } = string.Empty;
    public static string FullBasePath => Path.Combine(BasePath, DatabaseFileName);

    private readonly CelebrityContext _context;

    private Repository(string basePath)
    {
        string solutionRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        BasePath = Path.Combine(solutionRoot, basePath);
        Directory.CreateDirectory(BasePath);

        _context = new CelebrityContext(FullBasePath);
        _context.Database.EnsureCreated();
    }

    public static ICelebrity<Celebrity> Create(string basePath)
    {
        return new Repository(basePath);
    }

    public List<Celebrity> GetAllCelebrities()
    {
        return _context.Celebrities.AsNoTracking().OrderBy(c => c.Id).ToList();
    }

    public Celebrity? GetCelebrityById(int id)
    {
        return _context.Celebrities.AsNoTracking().FirstOrDefault(c => c.Id == id);
    }

    public bool DelCelebrity(int id)
    {
        Celebrity? celebrity = _context.Celebrities.FirstOrDefault(c => c.Id == id);
        if (celebrity is null)
            return false;

        _context.Celebrities.Remove(celebrity);
        return SaveChanges() > 0;
    }

    public bool AddCelebrity(Celebrity celebrity)
    {
        celebrity.Id = 0;
        _context.Celebrities.Add(celebrity);
        return _context.SaveChanges() > 0;
    }

    public int AddCelebrityAndGetId(Celebrity celebrity)
    {
        AddCelebrity(celebrity);
        _context.SaveChanges();
        return celebrity.Id;
    }

    public bool UpdCelebrity(int id, Celebrity celebrity)
    {
        Celebrity? current = _context.Celebrities.FirstOrDefault(c => c.Id == id);
        if (current is null)
            return false;

        current.Firstname = celebrity.Firstname;
        current.Surname = celebrity.Surname;
        current.PhotoPath = celebrity.PhotoPath;
        return SaveChanges() > 0;
  
    }

    public int GetCelebrityIdByName(string name)
    {
        Celebrity? c = _context.Celebrities.FirstOrDefault(c => c.Surname.Equals(name));
        return c != null ? c.Id : -1;
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
