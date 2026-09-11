# Структура проекта SportNutritionShop лабораторной работы 8

Этот файл объясняет, где находятся основные части проекта, что хранится в каждом файле и как компоненты связаны между собой.

## Общая схема работы

1. `App.xaml` подключает темы, стили, локализацию и конвертеры.
2. `App.xaml.cs` запускает создание или проверку SQLite-базы и открывает окно авторизации.
3. `LoginWindow` проверяет логин через `AuthService` и открывает главное окно.
4. `MainWindow` получает данные и команды из `MainViewModel`.
5. `MainViewModel` управляет каталогом, фильтрами, корзиной, заказами и Undo/Redo.
6. `DataService`, `ProfileService` и `AuthService` предоставляют специализированный доступ к данным.
7. `DatabaseService` выполняет настоящие ADO.NET-команды и транзакции SQLite.
8. Администратор может открыть `DatabaseWindow` для работы с таблицами через `DatabaseAdminViewModel`.

## Файлы в корне lab8

- `SportNutritionShop.sln` — решение Visual Studio 2026. Его удобнее всего открывать для запуска и сборки всей лабораторной.
- `PROJECT_STRUCTURE.md` — текущая документация по структуре проекта.
- `SportNutritionShop/` — папка самого WPF-проекта.

## Основные файлы проекта

- `SportNutritionShop/SportNutritionShop.csproj` — настройки сборки. Указывает WPF, `.NET 10.0 Windows`, иконку приложения, копируемые ресурсы и NuGet-пакеты `Microsoft.Data.Sqlite` и `SQLitePCLRaw`.
- `SportNutritionShop/App.config` — строка подключения SQLite, лимит малого остатка и таймаут команд базы данных.
- `SportNutritionShop/App.xaml` — общие ресурсы приложения. Подключает текущую тему, стили элементов управления, русский язык и конвертеры.
- `SportNutritionShop/App.xaml.cs` — точка запуска приложения. Инициализирует базу через `DatabaseService.InitializeAsync()`, обрабатывает ошибку создания БД и показывает `LoginWindow`.
- `SportNutritionShop/README.txt` — инструкция запуска, учетные записи, структура БД и соответствие требованиям лабораторной работы.

## Папка Data

- `Data/Schema.sql` — SQL-сценарий создания базы. Содержит таблицы `Categories`, `Users`, `Products`, `Orders`, `OrderItems`, `AuditLog`, `StoredCommands`, внешние ключи, представление `vw_ProductCatalog` и триггеры аудита товаров.
- `Data/SportNutritionShop.db` — рабочая SQLite-база. Создается автоматически не в исходной папке, а рядом с собранной программой: `bin/Debug/net10.0-windows/Data/`.
- `Data/ImageCache/` — создаваемый во время работы кэш изображений, извлеченных из BLOB-поля `Products.ImageData`.

## Папка Models

Модели описывают данные предметной области и не выполняют SQL-запросы.

- `Models/Product.cs` — товар: названия, описание, категория, бренд, страна, вкус, вес, цена, скидка, остаток, рейтинг, количество продаж и путь к картинке. Также вычисляет итоговую цену и признак отсутствия на складе.
- `Models/CartItem.cs` — позиция корзины. Связывает товар с выбранным количеством и вычисляет сумму позиции.
- `Models/SavedCart.cs` — сохраненный снимок корзины с названием и списком позиций.
- `Models/UserProfile.cs` — редактируемые данные профиля: имя, фамилия, электронная почта и телефон.
- `Models/UserRole.cs` — перечисление ролей `Client` и `Admin`.
- `Models/CategoryOption.cs` — элемент выпадающего списка категорий в окне базы данных. Хранит ID и отображаемое название категории.
- `Models/DatabaseTableInfo.cs` — описание таблицы для административного интерфейса: системное имя, отображаемое имя и разрешение редактирования.

## Папка Services

- `Services/DatabaseService.cs` — главный слой ADO.NET. Читает конфигурацию, открывает `SqliteConnection`, создает и заполняет БД, выполняет CRUD, параметризованные и асинхронные запросы, транзакции, оформление заказа, работу с BLOB-картинками и именованными SQL-командами.
- `Services/DataService.cs` — адаптер каталога и заказов для `MainViewModel`. Загружает и сохраняет товары, удаляет товар и создает заказ, не блокируя WPF-поток.
- `Services/AuthService.cs` — проверяет логин и пароль через таблицу `Users`, возвращает роль пользователя.
- `Services/ProfileService.cs` — загружает и сохраняет профиль пользователя в таблице `Users`.
- `Services/LocalizationService.cs` — во время работы заменяет словарь строк и переключает язык RU/EN.
- `Services/ThemeService.cs` — заменяет словарь цветов и переключает тему оформления.

## Папка ViewModels

- `ViewModels/MainViewModel.cs` — основная логика главного окна: каталог, поиск, фильтрация, сортировка, выбор товара, корзина, оформление заказа, административное добавление и удаление товаров, сохраненные корзины, переключение языка и Undo/Redo.
- `ViewModels/DatabaseAdminViewModel.cs` — логика окна базы данных: переход между таблицами, загрузка в `DataGrid`, добавление и удаление строк, сохранение изменений одной транзакцией, сортировка, запрос малого остатка и запуск `sp_ProductStatistics`.

## Папка Views

Каждое окно состоит из XAML-разметки и связанного файла C# с обработчиками событий.

- `Views/LoginWindow.xaml` — внешний вид окна авторизации.
- `Views/LoginWindow.xaml.cs` — получает логин и пароль, вызывает `AuthService`, открывает главное окно либо показывает ошибку.
- `Views/MainWindow.xaml` — разметка основного магазина: шапка, поиск, фильтры, карточки товаров, корзина, административная форма и кнопки Undo/Redo.
- `Views/MainWindow.xaml.cs` — открывает профиль и базу данных, выполняет выход, обрабатывает сохранение корзины и демонстрационные routed events.
- `Views/ProfileWindow.xaml` — форма профиля, выбора языка и темы.
- `Views/ProfileWindow.xaml.cs` — загружает профиль, сохраняет изменения, переключает тему и локализацию.
- `Views/DatabaseWindow.xaml` — административный `DataGrid`, кнопки перехода между таблицами, CRUD, сортировка и запуск запросов.
- `Views/DatabaseWindow.xaml.cs` — загружает данные, создает колонку выбора категории, скрывает пароли/BLOB/SQL-текст, завершает редактирование строки перед транзакцией.

## Папка Helpers

- `Helpers/BaseViewModel.cs` — базовый класс ViewModel. Реализует `INotifyPropertyChanged`, чтобы изменения свойств автоматически отражались в интерфейсе.
- `Helpers/RelayCommand.cs` — реализация `ICommand` для привязки кнопок к методам ViewModel и обновления их доступности.
- `Helpers/AppCommands.cs` — пользовательская маршрутизируемая команда сохранения корзины с сочетанием `Ctrl+S`.
- `Helpers/IUndoableAction.cs` — контракт обратимого действия с методами `Do()` и `Undo()`.
- `Helpers/UndoableAction.cs` — конкретное обратимое действие, построенное из двух делегатов: выполнить и отменить.
- `Helpers/UndoRedoManager.cs` — хранит стеки Undo и Redo, выполняет, отменяет и повторяет действия, уведомляет кнопки об изменении истории.

## Папка Converters

Конвертеры используются в XAML-привязках.

- `Converters/BoolToVisibilityConverter.cs` — `true` преобразует в `Visible`, `false` в `Collapsed`.
- `Converters/InverseBoolToVisibilityConverter.cs` — обратный вариант: `false` показывает элемент, `true` скрывает.
- `Converters/ZeroToCollapsedConverter.cs` — скрывает элемент при нулевом числовом значении; используется, например, для старой цены без скидки.

## Папка UserControls

- `UserControls/PriceRangeSlider.xaml` — разметка пользовательского элемента выбора минимальной и максимальной цены.
- `UserControls/PriceRangeSlider.xaml.cs` — свойства зависимости `MinPrice`/`MaxPrice`, проверка диапазона и события Tunnel/Bubble.
- `UserControls/RatingStars.xaml` — шаблон визуального отображения звезд рейтинга.
- `UserControls/RatingStars.xaml.cs` — свойство зависимости `Rating`, проверка диапазона 0–5, построение звезд и Direct routed event.

## Папка Resources

### Languages

- `Resources/Languages/Strings.ru-RU.xaml` — все русские подписи, сообщения, названия кнопок и полей.
- `Resources/Languages/Strings.en-US.xaml` — английские варианты тех же строк.

### Styles

- `Resources/Styles/Controls.xaml` — общие стили окон, карточек, кнопок, полей, списков, ComboBox, RadioButton и полос прокрутки. Здесь же находятся шаблоны, DataTrigger, MultiTrigger и анимации.

### Themes

- `Resources/Themes/Theme.Classic.xaml` — классическая синяя цветовая схема.
- `Resources/Themes/Theme.Optimistic.xaml` — яркая оптимистичная схема.
- `Resources/Themes/Theme.Pink.xaml` — розовая схема.
- `Resources/Themes/Theme.Grayscale.xaml` — черно-белая схема.

## Папка Assets

- `Assets/App.ico` — иконка приложения и окон.
- `Assets/Cursors/shop.cur` — пользовательский курсор магазина, используемый окнами.
- `Assets/Cursors/shop.png` — исходное PNG-изображение курсора.
- `Assets/Products/creatine.jpg` — начальное изображение креатина.
- `Assets/Products/whey.jpg` — начальное изображение протеина.
- `Assets/Products/bcaa.jpg` — начальное изображение BCAA.
- `Assets/Products/gainer.jpg` — начальное изображение гейнера.
- `Assets/Products/vitamins.jpg` — начальное изображение витаминов.
- `Assets/Products/preworkout.jpg` — начальное изображение предтренировочного комплекса.

При первом заполнении базы изображения из `Assets/Products` считываются как массивы байтов и записываются в `Products.ImageData`.

## Служебные папки сборки

- `bin/` — готовые DLL, EXE, конфигурация, ресурсы и рабочая база данных. Создается командой Build или запуском из Visual Studio.
- `obj/` — промежуточные файлы компиляции, сгенерированный WPF-код и сведения NuGet.
- `.vs/` — персональные настройки Visual Studio, если эта папка появилась после открытия проекта.

Эти три папки не содержат основной исходный код. При проблемах сборки `bin` и `obj` можно удалить после остановки приложения — Visual Studio создаст их заново.

## Где изменять нужную функцию

- Изменить работу каталога или корзины — `ViewModels/MainViewModel.cs`.
- Изменить SQL, структуру таблиц или транзакции — `Data/Schema.sql` и `Services/DatabaseService.cs`.
- Изменить административную таблицу — `Views/DatabaseWindow.xaml` и `ViewModels/DatabaseAdminViewModel.cs`.
- Изменить внешний вид главного окна — `Views/MainWindow.xaml`.
- Изменить цвета — нужный файл в `Resources/Themes`.
- Изменить общие стили кнопок и полей — `Resources/Styles/Controls.xaml`.
- Изменить подписи интерфейса — файлы в `Resources/Languages`.
- Изменить авторизацию — `Services/AuthService.cs` и таблица `Users`.
- Изменить Undo/Redo — `Helpers/UndoRedoManager.cs`, `Helpers/UndoableAction.cs` и места создания действий в `MainViewModel.cs`.
