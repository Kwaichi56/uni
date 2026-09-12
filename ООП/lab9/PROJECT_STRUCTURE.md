# Структура лабораторной работы 9

- `SportNutritionShop.slnx` — решение .NET 10.0 с WPF-приложением.
- `SportNutritionShop/SportNutritionShop.csproj` — WPF-проект и пакет EF Core SQLite.
- `SportNutritionShop/App.config` — строка подключения и настройки.
- `SportNutritionShop/Data/ShopEntities.cs` — сущности Code First.
- `SportNutritionShop/Data/ShopDbContext.cs` — `DbSet`, Fluent API, связи.
- `SportNutritionShop/Services/DatabaseService.cs` — CRUD, LINQ, асинхронные
  запросы, транзакции, начальные данные.
- `SportNutritionShop/ViewModels/DatabaseAdminViewModel.cs` и
  `Views/DatabaseWindow.xaml` — административное окно и фильтры.
- Остальные `Models`, `Views`, `ViewModels`, `UserControls`, `Resources` и
  `Assets` продолжают приложение лабораторной работы 8.
- `ModelGeneration/CodeFirstSchema.sql` — SQL-схема, выданная EF Core из модели.
- `MODEL_GENERATION.md` — демонстрация Database First и Model First.
- `Answers_Lab9.md` — ответы на вопросы задания.
