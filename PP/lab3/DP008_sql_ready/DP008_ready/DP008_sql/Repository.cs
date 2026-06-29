using DP008_ICelebrity;
using Microsoft.EntityFrameworkCore;

namespace DP008_sql;

public sealed class Repository : ICelebrity<Celebrity>
{
    public static string DatabaseFileName = "celebrities.db";
    public static string BasePath { get; private set; } = string.Empty;
    public static string FullBasePath => Path.Combine(BasePath, DatabaseFileName);

    private readonly CelebrityContext _context;
    private int _nChanges;

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
        _context.SaveChanges();
        _nChanges++;
        return true;
    }

    public bool AddCelebrity(Celebrity celebrity)
    {
        _context.Celebrities.Add(new Celebrity
        {
            Firstname = celebrity.Firstname,
            Surname = celebrity.Surname,
            PhotoPath = celebrity.PhotoPath
        });
        _context.SaveChanges();
        _nChanges++;
        return true;
    }

    public int AddCelebrityAndGetId(Celebrity celebrity)
    {
        Celebrity entry = new Celebrity
        {
            Firstname = celebrity.Firstname,
            Surname = celebrity.Surname,
            PhotoPath = celebrity.PhotoPath
        };

        _context.Celebrities.Add(entry);
        _context.SaveChanges();
        _nChanges++;
        return entry.Id;
    }

    public bool UpdCelebrity(int id, Celebrity celebrity)
    {
        Celebrity? current = _context.Celebrities.FirstOrDefault(c => c.Id == id);
        if (current is null)
            return false;

        current.Firstname = celebrity.Firstname;
        current.Surname = celebrity.Surname;
        current.PhotoPath = celebrity.PhotoPath;
        _context.SaveChanges();
        _nChanges++;
        return true;
    }

    public int GetCelebrityIdByName(string name)
    {
        Celebrity? celebrity = _context.Celebrities
            .AsNoTracking()
            .AsEnumerable()
            .FirstOrDefault(c => $"{c.Firstname}|{c.Surname}".Contains(name, StringComparison.OrdinalIgnoreCase));

        return celebrity?.Id ?? -1;
    }

    public int SaveChanges()
    {
        int result = _nChanges;
        _nChanges = 0;
        return result;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
