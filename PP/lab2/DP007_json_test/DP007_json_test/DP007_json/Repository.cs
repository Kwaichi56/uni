using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using DP007_ICelebrity;

namespace DP007_json
{

    public class Celebrity
    {
        public int Id { get; set; } 
        public string Firstname { get; set; } 
        public string Surname { get; set; } 
        public string PhotoPath { get; set; } 

        public Celebrity() { }

        public Celebrity(int id, string firstname, string surname, string photoPath) 
        {
            Id = id;
            Firstname = firstname; 
            Surname = surname;
            PhotoPath = photoPath; 
        }
    }


    public class Repository : ICelebrity<Celebrity> 
    {
        public static string JSONFileName = "celebrities.json"; 
        public string BasePath { get; private set; } 
        public string FullBasePath 
        {
            get { return Path.Combine(BasePath, JSONFileName); }
        }

        private List<Celebrity> celebrities; 
        private int NChanges = 0; 

        public static ICelebrity<Celebrity> Create(string basepath)
        {
            return new Repository(basepath); 
        }


        private Repository(string basepath)
        {
            celebrities = new List<Celebrity>();

            string solutionRoot =
                Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            BasePath = Path.Combine(solutionRoot, basepath);

            if (File.Exists(FullBasePath))
            {
                FileStream fs = new FileStream(FullBasePath, FileMode.Open, FileAccess.Read);
                try
                {
                    DataContractJsonSerializer ser =
                        new DataContractJsonSerializer(typeof(List<Celebrity>));
                    object obj = ser.ReadObject(fs);
                    List<Celebrity> list = obj as List<Celebrity>;
                    if (list != null)
                        celebrities = list;
                }
                finally
                {
                    fs.Close();
                }
            }
        }


        public List<Celebrity> GetAllCelebrities() 
        {
            return celebrities.ToList();
        }

        public bool AddCelebrity(Celebrity celebrity) 
        {
            int id = celebrities.Count > 0 ? celebrities.Max(c => c.Id) + 1 : 1;
            celebrities.Add(new Celebrity(id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath));
            NChanges++;
            return true;
        }

        public int AddCelebrityAndGetId(Celebrity celebrity)
        {
            int id = celebrities.Count > 0 ? celebrities.Max(c => c.Id) + 1 : 1;
            celebrities.Add(new Celebrity(id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath));
            NChanges++;
            return id;
        }

        public bool UpdCelebrity(int id, Celebrity celebrity)
        {
            int idx = celebrities.FindIndex(c => c.Id == id);
            if (idx >= 0)
            {
                celebrities[idx] = new Celebrity(id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
                NChanges++;
                return true;
            }
            return false;
        }

        public bool DelCelebrity(int id)
        {
            int idx = celebrities.FindIndex(c => c.Id == id);
            if (idx >= 0)
            {
                celebrities.RemoveAt(idx);
                NChanges++;
                return true;
            }
            return false;
        }

        public Celebrity GetCelebrityById(int id)
        {
            return celebrities.FirstOrDefault(c => c.Id == id);
        }

        public int GetCelebrityIdByName(string name)
        {
            Celebrity celebrity =
                celebrities.FirstOrDefault(c => (c.Firstname + "|" + c.Surname).Contains(name));
            if (celebrity != null)
                return celebrity.Id;
            return -1;
        }


        public int SaveChanges()
        {
            int rc = NChanges;
            if (rc > 0)
            {
                Directory.CreateDirectory(BasePath);

                FileStream fs = new FileStream(FullBasePath, FileMode.Create, FileAccess.Write);
                try
                {
                    DataContractJsonSerializer ser =
                        new DataContractJsonSerializer(typeof(List<Celebrity>));
                    ser.WriteObject(fs, celebrities);
                }
                finally
                {
                    fs.Close();
                }
            }
            NChanges = 0;
            return rc;
        }

        public void Dispose()
        {

        }   
    }
}
