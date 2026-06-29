using System;
using System.Collections.Generic;

namespace DP007_ICelebrity
{
    public interface ICelebrity<T> : IDisposable 
    {
        List<T> GetAllCelebrities(); 
        T GetCelebrityById(int id);
        bool DelCelebrity(int id);
        bool AddCelebrity(T celebrity); 
        int AddCelebrityAndGetId(T celebrity); 
        bool UpdCelebrity(int id, T celebrity); 
        int GetCelebrityIdByName(string name); 
        int SaveChanges();
    }
}
