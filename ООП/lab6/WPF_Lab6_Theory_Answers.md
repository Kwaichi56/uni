# Лабораторная работа №6: WPF — ресурсы, стили, триггеры, шаблоны, UserControl, Undo/Redo

## Теоретические вопросы и ответы

---

### 1. Для чего в WPF используются ресурсы? Каким образом можно определить новый ресурс и управлять им? Опишите назначение класса `ResourceDictionary`. Зачем каждый элемент имеет собственную коллекцию ресурсов?

#### Назначение ресурсов

**Ресурс** в WPF — это любой переиспользуемый объект (кисть, шрифт, стиль, шаблон, строка, геометрия, толщина и т. д.), определённый в XAML и доступный по ключу. Ресурсы решают три важные задачи:

1. **Переиспользование** — один и тот же объект (например, кисть `PrimaryBrush`) можно применить к десяткам элементов, не дублируя код.
2. **Централизованное управление** — чтобы поменять цвет акцента во всём приложении, достаточно изменить одно определение ресурса.
3. **Разделение логики и оформления** — внешний вид выносится из разметки элементов в отдельные словари, что упрощает поддержку и темизацию.

#### Способы определения ресурса

Ресурс определяется как XAML-элемент внутри коллекции `Resources` любого `FrameworkElement`, `Application` или отдельного `ResourceDictionary`. Обязателен атрибут `x:Key` — уникальное имя ресурса в рамках словаря.

```xml
<!-- Локальный ресурс на уровне окна -->
<Window.Resources>
    <Thickness x:Key="WindowStandardMargin">20</Thickness>
    <SolidColorBrush x:Key="LocalAccentBrush" Color="#1267C4"/>
</Window.Resources>

<!-- Использование -->
<Grid Margin="{StaticResource WindowStandardMargin}">
    <TextBlock Foreground="{StaticResource LocalAccentBrush}" Text="Привет"/>
</Grid>
```

#### Назначение класса `ResourceDictionary`

`ResourceDictionary` — это хеш-таблица, которая хранит ресурсы по ключам. У каждого `FrameworkElement` есть свойство `Resources` типа `ResourceDictionary`. У `Application` также есть `Resources` — это уже глобальный словарь, доступный из любого места приложения.

Ключевые возможности `ResourceDictionary`:

- **`MergedDictionaries`** — позволяет подключать внешние `.xaml`-файлы с ресурсами. Так организуется модульность: тема, стили, локализация выносятся в отдельные файлы.
- **`Source`** — задаёт путь к внешнему файлу словаря.
- **`ThemeDictionaries`** — используется для определения разных ресурсов под разные темы (Light/Dark).

Пример из проекта `SportNutritionShop`:

```xml
<!-- App.xaml -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Styles/Themes/LightTheme.xaml"/>
            <ResourceDictionary Source="Resources/Styles/Controls.xaml"/>
            <ResourceDictionary Source="Resources/Languages/Strings.ru-RU.xaml"/>
        </ResourceDictionary.MergedDictionaries>
        <conv:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter"/>
    </ResourceDictionary>
</Application.Resources>
```

#### Зачем у каждого элемента своя коллекция ресурсов?

Каждый `FrameworkElement` имеет свою коллекцию `Resources`, потому что **область видимости ресурса определяется его расположением**. Это даёт гибкость:

1. **Глобальные ресурсы** (в `Application.Resources`) — доступны во всём приложении: кисти тем, глобальные конвертеры.
2. **Ресурсы окна** (в `Window.Resources`) — видны только внутри этого окна: например, специфичный для окна отступ.
3. **Ресурсы элемента** (в `Button.Resources`) — видны только внутри этого элемента и его потомков: например, шаблон, применяемый только к этой кнопке.

Поиск ресурса идёт **снизу вверх**: сначала в коллекции текущего элемента, затем у его родителя, и так до `Application.Resources`. Это похоже на область видимости переменных в языках программирования — внутренние определения перекрывают внешние.

---

### 2. Какая разница между статическими и динамическими ресурсами?

WPF поддерживает два расширения разметки для ссылки на ресурсы: `StaticResource` и `DynamicResource`.

| Характеристика | `StaticResource` | `DynamicResource` |
|---|---|---|
| **Когда разрешается** | Один раз при загрузке XAML | При каждом обращении (с отслеживанием изменений) |
| **Реакция на замену ресурса** | Не реагирует — значение остаётся старым | Автоматически обновляется при смене ресурса в словаре |
| **Производительность** | Быстрее (одноразовое разрешение) | Медленнее (постоянный поиск и подписки) |
| **Синтаксис** | `{StaticResource Key}` | `{DynamicResource Key}` |
| **Типичные применения** | Кисти, толщины, геометрии, конвертеры — то, что не меняется в рантайме | Темы, локализация — то, что может быть переключено во время работы приложения |

#### Когда использовать `StaticResource`

Если ресурс создаётся один раз и не заменяется в рантайме. Например:

```xml
<Grid Margin="{StaticResource WindowStandardMargin}">  <!-- толщина 20 -->
```

#### Когда использовать `DynamicResource`

Если ресурс может быть заменён во время работы приложения. Например, смена темы:

```xml
<Window Background="{DynamicResource BackgroundBrush}">
<TextBlock Foreground="{DynamicResource PrimaryBrush}" Text="SPORT"/>
```

В проекте `SportNutritionShop` при переключении темы (Light → Green → Pink) код просто заменяет словарь темы в `Application.Resources.MergedDictionaries`. Так как все ссылки сделаны через `DynamicResource`, UI мгновенно перекрашивается без перезапуска приложения:

```csharp
// ThemeService.cs
public static void ApplyTheme(string themeName)
{
    var dictionaries = Application.Current.Resources.MergedDictionaries;
    // Заменяем первый словарь (тему) на новый
    dictionaries[0] = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };
}
```

А `StaticResource` для конвертеров работает быстрее — они не меняются в рантайме.

---

### 3. Что такое триггеры? Для чего в WPF используются триггеры? Назовите основные типы триггеров.

#### Что такое триггер

**Триггер** — это декларативное правило вида «когда выполняется условие, применить набор `Setter`». Триггеры позволяют менять свойства элементов **без написания code-behind** — реактивное поведение описывается прямо в XAML. Это базовый механизм для hover-эффектов, валидации, выделения и других визуальных состояний.

#### Назначение триггеров

- Реакция на наведение мыши, фокус, нажатие, выбор в списке
- Условное форматирование (например, зачёркивание цены при скидке)
- Анимации по событиям
- Изменение внешнего вида в зависимости от состояния или данных

#### Основные типы триггеров

| Тип | Базируется на | Что проверяет | Где определяется |
|---|---|---|---|
| `Trigger` (Property Trigger) | Свойствах **зависимостей** самого элемента | Значение свойства элемента (IsMouseOver, IsPressed, IsSelected) | В `Style.Triggers` или `ControlTemplate.Triggers` |
| `MultiTrigger` | Нескольких свойствах элемента | Одновременное выполнение нескольких условий | В `Style.Triggers` |
| `DataTrigger` | Привязанном **значении данных** | Значение свойства источника данных (DiscountPercent == 0) | В `Style.Triggers` |
| `MultiDataTrigger` | Нескольких привязанных значениях | Одновременное выполнение нескольких условий по данным | В `Style.Triggers` |
| `EventTrigger` | **Событии** элемента | Наступление события (MouseEnter, Click) | В `Style.Triggers`, `ControlTemplate.Triggers`, `FrameworkElement.Triggers` |

#### Примеры из проекта SportNutritionShop

**Property Trigger** — на свойство элемента:

```xml
<Style TargetType="TextBox">
    <Style.Triggers>
        <Trigger Property="IsKeyboardFocused" Value="True">
            <Setter Property="BorderBrush" Value="{DynamicResource PrimaryBrush}"/>
            <Setter Property="BorderThickness" Value="2"/>
        </Trigger>
    </Style.Triggers>
</Style>
```

**MultiTrigger** — на несколько свойств одновременно (фокус + наведение):

```xml
<MultiTrigger>
    <MultiTrigger.Conditions>
        <Condition Property="IsKeyboardFocused" Value="True"/>
        <Condition Property="IsMouseOver" Value="True"/>
    </MultiTrigger.Conditions>
    <Setter Property="BorderBrush" Value="{DynamicResource PrimaryBrush}"/>
    <Setter Property="BorderThickness" Value="2"/>
</MultiTrigger>
```

**DataTrigger** — на значение свойства данных:

```xml
<Style x:Key="DiscountPriceStyle" TargetType="TextBlock">
    <Style.Triggers>
        <DataTrigger Binding="{Binding DiscountPercent}" Value="0">
            <Setter Property="Visibility" Value="Collapsed"/>
        </DataTrigger>
        <DataTrigger Binding="{Binding DiscountPercent}" Value="100">
            <Setter Property="Foreground" Value="{DynamicResource DangerBrush}"/>
            <Setter Property="FontWeight" Value="Bold"/>
        </DataTrigger>
    </Style.Triggers>
</Style>
```

**EventTrigger** — на событие (запускает анимацию):

```xml
<EventTrigger RoutedEvent="Button.MouseEnter">
    <BeginStoryboard>
        <Storyboard>
            <DoubleAnimation
                Storyboard.TargetProperty="(UIElement.RenderTransform).(ScaleTransform.ScaleX)"
                To="1.05" Duration="0:0:0.25"/>
        </Storyboard>
    </BeginStoryboard>
</EventTrigger>
```

---

### 4. Что такое локализация и как её обеспечить

#### Определение

**Локализация** — это адаптация приложения под язык и культурные особенности конкретного региона: перевод текстов, форматы дат/чисел/валют, направление письма, изображения. В WPF локализация обычно сводится к переводу строк интерфейса, потому что форматирование чисел берёт на себя `CultureInfo` текущего потока.

#### Способы локализации в WPF

1. **Через `ResourceDictionary`** (используется в проекте) — для каждого языка создаётся отдельный `.xaml`-файл с одинаковыми ключами, но разными значениями. В рантайме нужный словарь подключается через `MergedDictionaries`.
2. **Через `.resx`-файлы** — стандартный механизм .NET (`Resources.resx`, `Resources.ru-RU.resx`). Доступ через `Properties.Resources.KeyName`.
3. **Через `x:Static`** — статические свойства из ресурсных сборок.
4. **Через `IValueConverter` с `CultureInfo`** — для форматирования чисел и дат.

#### Реализация в проекте SportNutritionShop

В проекте используется первый способ. Создано два файла:

```
Resources/Languages/
├── Strings.ru-RU.xaml   ← русский
└── Strings.en-US.xaml   ← английский
```

Каждый файл содержит одинаковые ключи:

```xml
<!-- Strings.ru-RU.xaml -->
<sys:String x:Key="CatalogTitle">Каталог товаров</sys:String>
<sys:String x:Key="AddToCartButton">В корзину</sys:String>
<sys:String x:Key="OutOfStock">Нет в наличии</sys:String>

<!-- Strings.en-US.xaml -->
<sys:String x:Key="CatalogTitle">Product catalog</sys:String>
<sys:String x:Key="AddToCartButton">Add to cart</sys:String>
<sys:String x:Key="OutOfStock">Out of stock</sys:String>
```

В XAML строки подключаются через `DynamicResource` (чтобы смена языка работала в рантайме):

```xml
<TextBlock Text="{DynamicResource CatalogTitle}"/>
<Button Content="{DynamicResource AddToCartButton}"/>
```

Смена языка в рантайме выполняет `LocalizationService`:

```csharp
public static void ChangeLanguage(string culture)
{
    var dictionaries = Application.Current.Resources.MergedDictionaries;
    // Находим словарь локализации (третий в коллекции) и заменяем его
    dictionaries[2] = new ResourceDictionary
    {
        Source = new Uri($"Resources/Languages/Strings.{culture}.xaml", UriKind.Relative)
    };
}
```

Преимущества подхода: мгновенное переключение языка без перезапуска, централизованное хранение переводов, лёгкое добавление новых языков (просто создаём новый `.xaml`-файл).

---

### 5. Что такое тема? Опишите процесс создания темы на основе ресурсов и стилей.

#### Что такое тема

**Тема** — это согласованный набор визуальных ресурсов (цветов, кистей, шрифтов, стилей), который задаёт общее оформление приложения. В WPF темы реализуются через `ResourceDictionary` с ресурсами-кистями и подключаются через `MergedDictionaries`. Смена темы — это замена словаря в `Application.Resources`, после чего все элементы, использующие `DynamicResource`, перекрашиваются автоматически.

#### Процесс создания темы

**Шаг 1. Спроектировать палитру** — определить набор цветов: основной (`Primary`), тёмный основной (`PrimaryDark`), фон (`Background`), карточка (`Card`), границы (`Border`), текст-заглушка (`Muted`), опасность (`Danger`), успех (`Success`).

**Шаг 2. Создать отдельный `.xaml`-файл темы** в папке `Resources/Styles/Themes/`. Каждый цвет объявляется как `Color`, а каждое использование — как `SolidColorBrush` (кисти можно менять в рантайме, а `Color` — нельзя):

```xml
<!-- LightTheme.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Color x:Key="PrimaryColor">#1267C4</Color>
    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource PrimaryColor}"/>
    <SolidColorBrush x:Key="PrimaryDarkBrush" Color="#0E4F97"/>
    <SolidColorBrush x:Key="BackgroundBrush" Color="#F3F6FA"/>
    <SolidColorBrush x:Key="CardBrush" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="BorderBrush" Color="#D8E0EA"/>
    <SolidColorBrush x:Key="MutedBrush" Color="#64748B"/>
    <SolidColorBrush x:Key="DangerBrush" Color="#D9534F"/>
    <SolidColorBrush x:Key="SuccessBrush" Color="#17A768"/>
</ResourceDictionary>
```

**Шаг 3. Создать другие темы** с теми же ключами, но разными цветами. Например, `GreenTheme.xaml` с зелёной палитрой и `PinkTheme.xaml` с розовой.

**Шаг 4. Подключить тему по умолчанию в `App.xaml`** как первый элемент `MergedDictionaries`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/Styles/Themes/LightTheme.xaml"/>
            <ResourceDictionary Source="Resources/Styles/Controls.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

**Шаг 5. Использовать кисти через `DynamicResource`** во всех стилях и разметке:

```xml
<Style TargetType="Button">
    <Setter Property="Background" Value="{DynamicResource PrimaryBrush}"/>
    <Setter Property="Foreground" Value="White"/>
</Style>
```

**Шаг 6. Реализовать сервис смены темы**, который заменяет словарь в рантайме:

```csharp
public static class ThemeService
{
    public const string LightTheme = "Light";
    public const string PinkTheme = "Pink";
    public const string GreenTheme = "Green";

    public static void ApplyTheme(string themeName)
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        dictionaries[0] = new ResourceDictionary
        {
            Source = new Uri($"Resources/Styles/Themes/{themeName}Theme.xaml", UriKind.Relative)
        };
    }
}
```

**Шаг 7. Сохранять выбор пользователя** в профиле (`ProfileService`), чтобы при следующем запуске применялась сохранённая тема.

---

### 6. Что такое шаблон и как его создать?

#### Что такое шаблон

**Шаблон (`ControlTemplate`)** — это XAML-описание визуального дерева элемента управления. По умолчанию каждый элемент управления (кнопка, текстовое поле, список) имеет стандартный шаблон, заданный системой. Создавая собственный `ControlTemplate`, можно полностью изменить внешний вид элемента, не меняя его логику.

Шаблон отличается от стиля: **стиль задаёт значения свойств** (`Background`, `FontSize`), а **шаблон задаёт структуру** (какие элементы внутри, их расположение, форма). Стиль может ссылаться на шаблон через `Setter Property="Template"`.

#### Виды шаблонов

- **`ControlTemplate`** — внешний вид элемента управления (кнопки, списка и т. д.).
- **`DataTemplate`** — внешний вид элемента данных (например, карточки товара в `ListBox`).
- **`ItemsPanelTemplate`** — панель, на которой раскладываются элементы `ItemsControl`.

#### Создание `ControlTemplate`

Шаблон определяется внутри `Style` через `Setter Property="Template"`. Внутри шаблона используются:

- **`ContentPresenter`** — место, где будет отображено содержимое (свойство `Content`).
- **`ItemsPresenter`** — место для элементов `ItemsControl`.
- **`TemplateBinding`** — связывает свойство элемента внутри шаблона со свойством самого контрола (например, `Background`).
- **`ControlTemplate.Triggers`** — триггеры, реагирующие на свойства контрола.

#### Пример: шаблон кнопки с rounded-формой

```xml
<Style TargetType="Button">
    <Setter Property="Background" Value="{DynamicResource PrimaryBrush}"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Border Background="{TemplateBinding Background}"
                        CornerRadius="10">
                    <ContentPresenter HorizontalAlignment="Center"
                                      VerticalAlignment="Center"/>
                </Border>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

#### Пример: шаблон кнопки с изменённой формой (Ellipse) из проекта

Этот шаблон **изменяет форму** кнопки с прямоугольной на круглую, что демонстрирует возможность полностью переписать визуальное представление:

```xml
<Style x:Key="RoundIconButtonStyle" TargetType="Button">
    <Setter Property="Width" Value="42"/>
    <Setter Property="Height" Value="42"/>
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate TargetType="Button">
                <Grid>
                    <Ellipse x:Name="Bd"
                             Fill="{TemplateBinding Background}"
                             Stroke="{DynamicResource BorderBrush}"
                             StrokeThickness="1"/>
                    <ContentPresenter HorizontalAlignment="Center"
                                      VerticalAlignment="Center"/>
                </Grid>
                <ControlTemplate.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter TargetName="Bd" Property="Fill"
                                Value="{DynamicResource PrimaryDarkBrush}"/>
                    </Trigger>
                    <Trigger Property="IsEnabled" Value="False">
                        <Setter TargetName="Bd" Property="Fill" Value="#9CA3AF"/>
                    </Trigger>
                </ControlTemplate.Triggers>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

#### Пример: `DataTemplate` для карточки товара

```xml
<ListBox.ItemTemplate>
    <DataTemplate DataType="{x:Type models:Product}">
        <Border Style="{StaticResource ProductCardStyle}">
            <TextBlock Text="{Binding ShortName}"/>
            <!-- ... -->
        </Border>
    </DataTemplate>
</ListBox.ItemTemplate>
```

---

### 7. Зачем нужны пользовательские элементы управления? Как создать собственный элемент? Опишите члены класса `UserControl`.

#### Зачем нужны UserControl

**Пользовательский элемент управления (`UserControl`)** — это составной элемент, объединяющий несколько существующих элементов в один переиспользуемый блок. Когда один и тот же фрагмент UI (например, звёздный рейтинг, адресная карточка, форма входа) используется в нескольких местах, его оформляют как `UserControl` и переиспользуют как обычный элемент.

Преимущества:
- **Инкапсуляция** — внутренняя структура скрыта, наружу выводятся только нужные свойства.
- **Переиспользование** — элемент можно вставить в любое место разметки одной строкой.
- **Изоляция логики** — код-behind (или ViewModel) элемента отделён от остального приложения.

#### Как создать собственный элемент

**Шаг 1. Создать файл `UserControl.xaml`** и его `xaml.cs`. В XAML описывается визуальная структура, в code-behind — логика (или используется MVVM).

**Шаг 2. Определить `DependencyProperty`** для свойств, которые должны быть доступны снаружи и поддерживать привязку данных.

**Шаг 3. Использовать элемент в разметке**, объявив пространство имён и вставив тег элемента.

#### Пример: `RatingControl` из проекта SportNutritionShop

`RatingControl` отображает рейтинг в виде звёзд. Внешний вид описан в XAML через `Path` с геометрией звезды, логика — в code-behind через `DependencyProperty`.

**XAML:**

```xml
<UserControl x:Class="SportNutritionShop.Controls.RatingControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel Orientation="Horizontal" x:Name="StarsPanel"/>
</UserControl>
```

**Code-behind с `DependencyProperty`:**

```csharp
public partial class RatingControl : UserControl
{
    // DependencyProperty — позволяет свойству участвовать в привязке данных
    public static readonly DependencyProperty RatingProperty =
        DependencyProperty.Register(
            name: nameof(Rating),
            propertyType: typeof(double),
            ownerType: typeof(RatingControl),
            typeMetadata: new PropertyMetadata(0.0, OnRatingChanged));

    public double Rating
    {
        get => (double)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }

    public int MaxRating { get; set; } = 5;

    public RatingControl()
    {
        InitializeComponent();
        RenderStars();
    }

    private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Вызывается при изменении Rating — перерисовываем звёзды
        (d as RatingControl)?.RenderStars();
    }

    private void RenderStars() { /* отрисовка звёзд */ }
}
```

**Использование в разметке:**

```xml
<controls:RatingControl Rating="{Binding Rating, Mode=OneWay}" MaxRating="5"/>
```

#### Члены класса `UserControl`

`UserControl` наследуется от `ContentControl`, который наследуется от `Control`, `FrameworkElement`, `UIElement`, `Visual`. Ключевые члены:

| Член | Тип | Назначение |
|---|---|---|
| `Content` | `object` | Содержимое элемента (обычно панель с дочерними элементами). |
| `ContentTemplate` | `DataTemplate` | Шаблон для отображения `Content`. |
| `DataContext` | `object` | Контекст данных для привязок внутри UserControl. |
| `Resources` | `ResourceDictionary` | Локальные ресурсы элемента. |
| `Triggers` | `TriggerCollection` | Триггеры, реагирующие на свойства и события. |
| `DependencyProperty` (статические) | — | Кастомные свойства, объявленные через `DependencyProperty.Register`. Только так свойство поддерживает привязку, анимацию, стилизацию. |
| `Loaded`, `Unloaded` | события | Возникают при загрузке/выгрузке элемента в визуальное дерево. |

Ключевая особенность: **чтобы свойство UserControl участвовало в привязке данных (`{Binding ...}`), оно должно быть объявлено как `DependencyProperty`** — обычное CLR-свойство не будет работать с XAML-привязкой.

---

### 8. Что такое привязка данных? В чём разница между режимами OneWay, TwoWay и OneTime?

#### Что такое привязка данных

**Привязка данных (data binding)** — это механизм автоматической синхронизации значения свойства источника (source) со свойством цели (target). Источником обычно выступает объект данных (модель, ViewModel), целью — свойство элемента управления (`Text`, `ItemsSource`, `Visibility`).

Привязка описывается в XAML через расширение разметки `{Binding}`:

```xml
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"/>
```

Здесь:
- `SearchText` — путь к свойству источника (по умолчанию — `DataContext` элемента).
- `UpdateSourceTrigger=PropertyChanged` — когда обновлять источник (при каждом нажатии клавиши, а не при потере фокуса).

Компоненты привязки:
- **Источник (source)** — объект с данными.
- **Цель (target)** — свойство зависимости элемента управления.
- **Путь (path)** — какое свойство источника привязывается.
- **Режим (mode)** — направление и частота обновлений.
- **Конвертер (converter)** — опциональный `IValueConverter` для преобразования типов.

#### Режимы привязки

| Режим | Направление | Когда обновляется | Типичное применение |
|---|---|---|---|
| **`OneWay`** | Источник → Цель | При изменении источника (через `INotifyPropertyChanged`) | Отображение данных, которые пользователь не редактирует: `TextBlock.Text = {Binding ProductName}` |
| **`TwoWay`** | Источник ↔ Цель | В обе стороны: при изменении источника И при изменении цели | Редактируемые поля: `TextBox.Text`, `CheckBox.IsChecked` |
| **`OneTime`** | Источник → Цель | Один раз при инициализации привязки | Статические данные, которые точно не изменятся: иконки, заголовки |

#### Подробное сравнение

**`OneWay`** — наиболее частый режим. Источник меняется → цель обновляется. Но если пользователь меняет значение в UI (например, вводит текст), источник не обновляется. Используется для отображения «только для чтения» данных:

```xml
<TextBlock Text="{Binding CartTotal, Mode=OneWay, StringFormat={}{0} BYN}"/>
```

**`TwoWay`** — двусторонняя синхронизация. Пользователь меняет значение в UI → источник обновляется (с учётом `UpdateSourceTrigger`). Программное изменение источника → UI обновляется. Это режим по умолчанию для редактируемых элементов (`TextBox.Text`, `CheckBox.IsChecked`, `ComboBox.SelectedItem`):

```xml
<TextBox Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"/>
<!-- Пользователь вводит текст → SearchText в ViewModel обновляется -->
<!-- VM меняет SearchText программно → TextBox.Text обновляется -->
```

**`OneTime`** — привязка читает значение один раз при создании элемента и больше не следит за изменениями. Это **самый быстрый режим**, потому что не нужно подписываться на `INotifyPropertyChanged`. Подходит для данных, которые не меняются после загрузки (например, ID записи, дата создания).

#### Дополнительные режимы

- **`OneWayToSource`** — обратный `OneWay`: цель → источник (применяется редко).
- **`Default`** — режим по умолчанию для свойства цели (зависит от метаданных свойства зависимости).

---

### 9. Объясните назначение интерфейса `INotifyPropertyChanged`

#### Назначение

Интерфейс `INotifyPropertyChanged` — это механизм уведомления UI об изменении свойств объекта-источника данных. Без реализации этого интерфейса привязки `OneWay` и `TwoWay` **не будут автоматически обновляться** при программном изменении свойства — UI останется со старым значением.

Интерфейс находится в пространстве имён `System.ComponentModel` и содержит одно событие:

```csharp
public interface INotifyPropertyChanged
{
    event PropertyChangedEventHandler? PropertyChanged;
}
```

#### Как это работает

1. Класс-источник данных реализует `INotifyPropertyChanged`.
2. В сеттере каждого свойства вызывается событие `PropertyChanged` с именем изменившегося свойства.
3. WPF-привязка автоматически подписывается на это событие.
4. При срабатывании события привязка перечитывает значение свойства и обновляет UI.

#### Базовая реализация

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // [CallerMemberName] автоматически подставляет имя вызывающего свойства
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

#### Использование в модели

```csharp
public class Product : BaseViewModel
{
    private decimal _price;

    public decimal Price
    {
        get => _price;
        set
        {
            _price = value;
            OnPropertyChanged();              // уведомляем об изменении Price
            OnPropertyChanged(nameof(FinalPrice));  // уведомляем зависимое свойство
        }
    }

    public decimal FinalPrice => Price - (Price * DiscountPercent / 100m);
    // FinalPrice пересчитывается автоматически при изменении Price
}
```

#### Зачем уведомлять зависимые свойства

Если свойство `B` вычисляется на основе свойства `A`, то при изменении `A` нужно дополнительно вызвать `OnPropertyChanged(nameof(B))`, иначе UI, привязанный к `B`, не обновится. В примере выше при изменении `Price` вызывается `OnPropertyChanged(nameof(FinalPrice))`, потому что `FinalPrice` зависит от `Price`, и `TextBlock`, привязанный к `FinalPrice`, должен перерисоваться.

Аналогично при изменении `Quantity` уведомляется `IsOutOfStock`:

```csharp
public int Quantity
{
    get => _quantity;
    set
    {
        _quantity = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(IsOutOfStock));  // кнопка "В корзину" скроется, если 0
    }
}

public bool IsOutOfStock => Quantity <= 0;
```

#### Важность для MVVM

В паттерне MVVM (Model-View-ViewModel) `INotifyPropertyChanged` — это **связующее звено между ViewModel и View**. ViewModel реализует этот интерфейс, View через привязки подписывается на его события, и любые изменения в ViewModel автоматически отражаются в UI без явного обращения к элементам управления из кода. Это сохраняет разделение логики и представления — главное преимущество MVVM.

Без `INotifyPropertyChanged`:
- Изменение свойства в коде не отразится в UI.
- Привязка `TwoWay` сработает один раз (от UI к источнику), но обратное обновление не произойдёт.
- Кнопки, доступность которых зависит от свойств (`CanExecute` в `ICommand`), не будут реагировать на изменения.

С реализацией интерфейса приложение становится **реактивным**: пользователь видит изменения данных мгновенно, как только они произошли в коде.
