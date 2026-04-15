using System;
using System.Collections.Generic;

namespace DP007_ICelebrity
{
    public interface ICelebrity<T> : IDisposable // интерфейс для работы с знаменитостями
    {
        List<T> GetAllCelebrities(); // получаем всех знаменитостей
        T GetCelebrityById(int id); // получаем знаменитость по ID
        bool DelCelebrity(int id); // удаляем знаменитость по ID
        bool AddCelebrity(T celebrity); // добавляем знаменитость
        int AddCelebrityAndGetId(T celebrity); // добавляем знаменитость и получаем её ID
        bool UpdCelebrity(int id, T celebrity); // обновляем знаменитость по ID
        int GetCelebrityIdByName(string name); // получаем ID знаменитости по имени
        int SaveChanges(); // сохраняем изменения
    }
}
