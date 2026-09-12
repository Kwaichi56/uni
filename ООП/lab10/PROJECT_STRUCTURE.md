# Структура лабораторной работы 10

- `SportNutritionShop.slnx` — решение .NET 10.0 с WPF-приложением.
- `SportNutritionShop/SportNutritionShop.csproj` — WPF-проект и пакет EF Core SQLite.
- `SportNutritionShop/App.config` — строка подключения и настройки.
- `SportNutritionShop/Data/ShopEntities.cs` — сущности Code First.
- `SportNutritionShop/Data/ShopDbContext.cs` — `DbSet`, Fluent API, связи.
- `SportNutritionShop/Data/IShopUnitOfWork.cs` — интерфейсы Repository и Unit of Work.
- `SportNutritionShop/Data/EfRepositories.cs` — реализация репозиториев на EF Core.
- `SportNutritionShop/Data/EfShopUnitOfWork.cs` — единый контекст, сохранение и транзакции.
- `SportNutritionShop/Services/DatabaseService.cs` — бизнес-операции и преобразование моделей UI.
- `SportNutritionShop/ViewModels/DatabaseAdminViewModel.cs` и
  `Views/DatabaseWindow.xaml` — административное окно и фильтры.
- Остальные `Models`, `Views`, `ViewModels`, `UserControls`, `Resources` и
  `Assets` продолжают приложение лабораторной работы 9.
- `ModelGeneration/CodeFirstSchema.sql` — SQL-схема, выданная EF Core из модели.
- `MODEL_GENERATION.md` — демонстрация Database First и Model First.
- `Answers_Lab10.md` — ответы на вопросы задания.
- `Verification` — проверка Repository, Unit of Work, отката транзакции и заказа.
