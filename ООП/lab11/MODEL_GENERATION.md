# Генерация модели из базы и базы из модели

Исполняемый проект использует EF Core. В EF Core модель — это классы сущностей,
`DbContext` и конфигурация Fluent API; дизайнерский файл `.edmx` им не создаётся.
Классический EDMX-дизайнер с режимами Database First и Model First относится к EF6.
Поэтому ниже приведены оба способа без смешения форматов.

## Database First из существующей базы

1. Запустить магазин хотя бы один раз, чтобы получить
   `SportNutritionShop/bin/Debug/net10.0-windows/Data/SportNutritionShop.db`.
2. Для EF Core создать отдельный учебный проект и установить инструмент
   `dotnet-ef`, провайдер SQLite и пакет `Microsoft.EntityFrameworkCore.Design`
   той же версии, что и основной пакет.
3. Из каталога `lab11` выполнить команды ниже. Абсолютный путь к БД нужно
   заменить, если решение находится в другом каталоге. Сгенерированные классы
   не заменяют действующую Code First модель.

```powershell
dotnet new classlib -n DatabaseFirstDemo -f net10.0
dotnet add DatabaseFirstDemo package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.4
dotnet add DatabaseFirstDemo package Microsoft.EntityFrameworkCore.Design --version 8.0.4
dotnet tool install --global dotnet-ef --version 8.0.4
dotnet ef dbcontext scaffold "Data Source=D:\BSTU\Labs\ООП\lab11\SportNutritionShop\bin\Debug\net10.0-windows\Data\SportNutritionShop.db" Microsoft.EntityFrameworkCore.Sqlite --project DatabaseFirstDemo --output-dir DatabaseFirstEntities --context ScaffoldedShopContext --no-onconfiguring --force
```

Результат — классы `Category`, `Product`, `User`, `Order` и других таблиц,
а также `ScaffoldedShopContext` с отображением колонок и связей. Это
reverse engineering схемы в модель EF Core, без `.edmx`.

Если в учебном задании требуется именно **EDM/EDMX**, создать отдельный проект
**.NET Framework + EF6** в Visual Studio: **Add → New Item → ADO.NET Entity Data
Model → EF Designer from database**. Подключить БД через поддерживаемый дизайнером
провайдер, выбрать таблицы и завершить мастер. Он создаст `.edmx`, контекст и
сущностные классы. Это отдельная демонстрация: EF6-дизайнер не является частью
приложения EF Core на .NET 10.

## Model First

В классическом EF6-дизайнере: **Add → New Item → ADO.NET Entity Data Model →
Empty EF Designer model**. Добавить сущности `Category`, `Product`, `User`,
`Order`, `OrderItem` и связи 1:M, затем выбрать **Generate Database from Model**.
Дизайнер создаст SQL-сценарий схемы и классы из `.edmx`.

Эквивалент «сначала классы, затем база» в этом проекте — Code First:
`ShopEntities.cs` → `ShopDbContext.OnModelCreating` → `EnsureCreatedAsync`.
Метод `GenerateCreateScript()` выдаёт SQL для той же модели без создания БД.
Полученный таким способом сценарий сохранён в
`ModelGeneration/CodeFirstSchema.sql`.

Полезные первичные руководства:
[EF Core Reverse Engineering](https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/),
[EF6 Database First](https://learn.microsoft.com/en-us/ef/ef6/modeling/designer/workflows/database-first),
[EF6 Model First](https://learn.microsoft.com/en-us/ef/ef6/modeling/designer/workflows/model-first).
