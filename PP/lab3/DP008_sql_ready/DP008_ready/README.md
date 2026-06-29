# Лабораторная работа №8 — Repository/SQL

Готовое решение содержит:
- `DP008_ICelebrity` — интерфейс `ICelebrity<T>`.
- `DP008_sql` — библиотеку репозитория на Entity Framework Core + SQLite.
- `DP008_sql_test` — консольный тест по образцу из лабораторной №7.
- `Database/init.sql` — SQL-скрипт для одной таблицы `Celebrities`.

## Что реализовано

Репозиторий поддерживает методы:
- `GetAllCelebrities()`
- `GetCelebrityById(int id)`
- `DelCelebrity(int id)`
- `AddCelebrity(Celebrity celebrity)`
- `AddCelebrityAndGetId(Celebrity celebrity)`
- `UpdCelebrity(int id, Celebrity celebrity)`
- `GetCelebrityIdByName(string name)`
- `SaveChanges()`

## База данных

Используется SQLite-файл `Database/celebrities.db`.
При создании репозитория база автоматически создаётся заново и наполняется 7 начальными записями:
Noam Chomsky, Tim Berners-Lee, Edgar Codd, Donald Knuth, Linus Torvalds, John Neumann, Edsgar Dijkstra.

Это сделано для того, чтобы тест каждый раз начинался с одинакового состояния и выдавал воспроизводимый результат, как требует задание.

## Как запустить

1. Открыть `DP008_sql.sln` в Visual Studio 2022 или через `dotnet` CLI.
2. Восстановить NuGet-пакеты.
3. Запустить проект `DP008_sql_test`.

## Используемые пакеты

- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Sqlite`

## Ожидаемая логика теста

1. Вывести всех знаменитостей.
2. Добавить `Marlin Minsky`.
3. Найти его по имени и обновить на `Marvin Minsky`.
4. Найти `Minsky` и удалить.
5. Снова добавить `Marvin Minsky`, получить его ID.
6. Сохранить изменения.
