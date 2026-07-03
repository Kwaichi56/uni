using DP007_ICelebrity;
using DP007_ICelebrity.lib;
using DP007_json.lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DP007_json
{
    public class Repository : ICelebrity<Celebrity>
    {
        public static string JSONFileName = "celebrities.json";
        public string BasePath { get; private set; }
        public string FullBasePath => Path.Combine(this.BasePath, JSONFileName);

        private List<Celebrity> celebrities = new();
        private int NChanges = 0;

        public static ICelebrity<Celebrity> Create(string basepath) => new Repository(basepath);

        private Repository(string basepath)
        {
            this.BasePath = Path.Combine(Directory.GetCurrentDirectory(), basepath);

            if (File.Exists(this.FullBasePath))
            {
                using FileStream fs = new FileStream(this.FullBasePath, FileMode.Open, FileAccess.Read);
                fs.Seek(0, SeekOrigin.Begin);

                var c = JsonSerializer.DeserializeAsync<List<Celebrity>>(fs).Result;
                if (c != null) this.celebrities = c;
            }
        }

        public List<Celebrity> GetAllCelebrities() => this.celebrities.ToList();

        public Celebrity? GetCelebrityById(int id) => this.celebrities.FirstOrDefault(c => c.Id == id);

        public bool AddCelebrity(Celebrity celebrity)
        {
            int id = this.celebrities.Count > 0 ? this.celebrities.Max(c => c.Id) + 1 : 1;
            this.celebrities.Add(celebrity with { Id = id });
            this.NChanges++;
            return true;
        }

        public int AddCelebrityAndGetId(Celebrity celebrity)
        {
            int id = this.celebrities.Count > 0 ? this.celebrities.Max(c => c.Id) + 1 : 1;
            this.celebrities.Add(celebrity with { Id = id });
            this.NChanges++;
            return id;
        }

        public bool UpdCelebrity(int id, Celebrity celebrity)
        {
            int ntx = this.celebrities.FindIndex(c => c.Id == id);
            if (ntx >= 0)
            {
                this.celebrities[ntx] = celebrity with { Id = id };
                this.NChanges++;
                return true;
            }
            return false;
        }

        public bool DelCelebrity(int id)
        {
            int ntx = this.celebrities.FindIndex(c => c.Id == id);
            if (ntx >= 0)
            {
                this.celebrities.RemoveAt(ntx);
                this.NChanges++;
                return true;
            }
            return false;
        }

        public int GetCelebrityIdByName(string name)
        {
            Celebrity? celebrity = this.celebrities.FirstOrDefault(c => (c.Firstname + "|" + c.Surname).Contains(name));
            return celebrity != null ? celebrity.Id : -1;
        }

        public int SaveChanges()
        {
            int rc = this.NChanges;
            if (rc > 0)
            {
                Directory.CreateDirectory(BasePath);
                using FileStream fs = new FileStream(this.FullBasePath, FileMode.Truncate, FileAccess.Write);
                fs.Seek(0, SeekOrigin.Begin);

                var options = new JsonSerializerOptions { WriteIndented = true };
                JsonSerializer.SerializeAsync(fs, this.celebrities, options).Wait();

                this.NChanges = 0;
            }
            return rc;
        }

        public void Dispose() { }
    }
}