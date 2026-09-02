# Ответы на контрольные вопросы (Лаб. №7)

## 1. Что такое свойства зависимости? Для чего они нужны?

**Свойство зависимости (DependencyProperty)** — это специальный вид свойства в WPF, значение которого хранится не в обычном приватном поле класса, а во внешнем хранилище, управляемом системой свойств WPF (через методы `GetValue` / `SetValue`, унаследованные от `DependencyObject`).

Регистрируется через статическое поле `DependencyProperty.Register(...)` и оборачивается обычным CLR-свойством:

```csharp
public static readonly DependencyProperty RatingProperty =
    DependencyProperty.Register(nameof(Rating), typeof(double), typeof(RatingStars),
        new FrameworkPropertyMetadata(0.0, OnRatingChanged, CoerceRating));

public double Rating
{
    get => (double)GetValue(RatingProperty);
    set => SetValue(RatingProperty, value);
}
```

**Для чего нужны:**

- **Поддержка привязки данных (Binding).** Обычные CLR-свойства не способны уведомлять привязку об изменениях автоматически. Свойства зависимости это делают через `PropertyMetadata`.
- **Стилизация и шаблоны.** Только DP могут быть заданы через `Setter` в `Style`, `ControlTemplate`, `DataTemplate`.
- **Анимация.** WPF анимирует именно DP (через `Storyboard.TargetProperty`).
- **Наследование значения.** DP могут получать значение от родительского элемента по дереву (например, `FontSize`, `Foreground`).
- **Значения по умолчанию.** Система свойств хранит их отдельно от экземпляра, экономя память.
- **Приоритет значений.** У DP есть строго определённая система приоритетов (анимация > стиль > локальное значение > значение по умолчанию), что недоступно обычным свойствам.
- **Уведомление об изменениях.** Через `PropertyChangedCallback` в `PropertyMetadata` можно реагировать на изменение без реализации `INotifyPropertyChanged`.

---

## 2. Как создать DependencyProperty?

Создаётся в 4 шага:

1. Класс-владелец должен наследоваться от `DependencyObject` (или его наследника — `UIElement`, `Control`, `UserControl` и т.д.).
2. Объявить статическое поле `public static readonly DependencyProperty`, зарегистрировав его через `DependencyProperty.Register(...)`.
3. Создать CLR-обёртку (get/set) с вызовом `GetValue` / `SetValue` — **без** дополнительной логики в аксессорах (иначе нарушится работа системы свойств WPF).
4. (По необходимости) передать `PropertyMetadata` с обработчиками изменения и коррекции значения.

```csharp
public class RatingStars : UserControl   // UserControl → DependencyObject
{
    // 1) Регистрация
    public static readonly DependencyProperty RatingProperty =
        DependencyProperty.Register(
            nameof(Rating),                 // имя (строка)
            typeof(double),                 // тип свойства
            typeof(RatingStars),            // тип владельца
            new FrameworkPropertyMetadata(  // метаданные
                0.0,                        // значение по умолчанию
                OnRatingChanged,            // PropertyChangedCallback
                CoerceRating),              // CoerceValueCallback
            ValidateRating);                // ValidateValueCallback

    // 2) CLR-обёртка — без логики, только GetValue/SetValue
    public double Rating
    {
        get => (double)GetValue(RatingProperty);
        set => SetValue(RatingProperty, value);
    }
}
```

**Важные правила:**

- Имя статического поля — обычно `<ИмяСвойства>Property` (по соглашению).
- В `Register` имя передаётся строкой и должно точно совпадать с именем CLR-свойства.
- В аксессорах `get/set` нельзя добавлять проверки или побочные эффекты — система свойств WPF вызывает внутренние механизмы в обход обёртки (например, при анимации или привязке).

---

## 3. Для чего и как используют делегат ValidateValueCallback?

**Назначение.** `ValidateValueCallback` — это делегат `bool ValidateValueCallback(object value)`, который проверяет **корректность** присваиваемого значения **на самом низком уровне** системы свойств, ещё до того, как значение попадёт в хранилище. Если колбэк возвращает `false`, WPF выбрасывает исключение `ArgumentException`, и значение свойства не меняется.

**Отличия от `CoerceValueCallback`:**

| Делегат | Что делает | Может изменить значение | Что если условие нарушено |
|---|---|---|---|
| `ValidateValueCallback` | Проверяет **допустимость** значения | Нет | Бросает исключение |
| `CoerceValueCallback` | **Корректирует** значение к допустимому диапазону | Да | Не падает, а возвращает скорректированное |

**Как использовать.** Передаётся 4-м параметром в перегрузку `DependencyProperty.Register(...)`:

```csharp
public static readonly DependencyProperty RatingProperty =
    DependencyProperty.Register(
        nameof(Rating), typeof(double), typeof(RatingStars),
        new FrameworkPropertyMetadata(0.0, OnRatingChanged, CoerceRating),
        ValidateRating);   // ← 5-й параметр

// Валидация: рейтинг только в диапазоне 0..5
private static bool ValidateRating(object value) =>
    value is double d && d >= 0 && d <= 5;
```

**Особенности:**

- Колбэк **статический** (не имеет доступа к экземпляру `DependencyObject`), поэтому проверяет только само значение, без контекста контрола.
- Применяется для «жёстких» ограничений — типов, диапазонов, форматов, при нарушении которых продолжать работу бессмысленно.
- Срабатывает при **любом** способе установки значения: локально, через стиль, анимацию, привязку.

---

## 4. Для чего и как используют делегат CoerceValueCallback?

**Назначение.** `CoerceValueCallback` — это делегат `object CoerceValueCallback(DependencyObject d, object baseValue)`, который **корректирует** значение, прежде чем оно будет сохранено. В отличие от валидации, он не отказывает в записи, а возвращает исправленное значение.

**Типичные применения:**

- Ограничение значения в диапазоне (clamp).
- Синхронизация связанных свойств (например, `MinPrice ≤ MaxPrice`).
- Приведение значения к каноническому виду (например, upper-case для строки).

**Как использовать.** Передаётся третьим параметром в `FrameworkPropertyMetadata`:

```csharp
public static readonly DependencyProperty RatingProperty =
    DependencyProperty.Register(
        nameof(Rating), typeof(double), typeof(RatingStars),
        new FrameworkPropertyMetadata(0.0, OnRatingChanged, CoerceRating),  // ← Coerce
        ValidateRating);

// Коррекция: ограничиваем значение в диапазоне 0..5
private static object CoerceRating(DependencyObject d, object baseValue)
{
    double v = (double)baseValue;
    if (v < 0) return 0.0;
    if (v > 5) return 5.0;
    return v;
}
```

**Пример взаимной коррекции (из `PriceRangeSlider`):** при изменении `MinPrice` проверяем, не превышает ли он `MaxPrice`, и если да — возвращаем текущий `MaxPrice`:

```csharp
private static object CoerceMinPrice(DependencyObject d, object baseValue)
{
    if (d is PriceRangeSlider c &&
        double.TryParse((string)baseValue, out var min) &&
        double.TryParse(c.MaxPrice, out var max) && min > max)
        return c.MaxPrice;     // ← корректируем
    return baseValue;
}
```

**Порядок срабатывания** при установке значения: `CoerceValueCallback` → `ValidateValueCallback` → сохранение → `PropertyChangedCallback`.

---

## 5. Какие типы маршрутизируемых событий есть в WPF (поясните каждый)?

В WPF определены три стратегии маршрутизации (перечисление `RoutingStrategy`):

### 5.1. Direct (прямая)

- Событие возникает **только на том элементе**, где оно было инициировано.
- Не поднимается и не опускается по визуальному дереву.
- Похоже на обычное CLR-событие, но использует инфраструктуру `RoutedEvent` (можно подписываться через `AddHandler`).
- Применяется для событий, не имеющих смысла у родителей: `MouseEnter`, `MouseLeave`, `TextChanged` у `TextBox`, `RatingChanged` у `RatingStars`.

### 5.2. Tunneling (туннелирование, «нисходящая»)

- Событие начинает идти **от корня визуального дерева вниз** к источнику.
- Имена туннельных событий по соглашению начинаются с префикса `Preview` (`PreviewMouseDown`, `PreviewKeyDown`).
- Используется для перехвата события до того, как оно достигнет источника, — например, чтобы отменить его (`e.Handled = true`) или выполнить подготовку.
- В нашем коде: `PreviewRangeChanged` в `PriceRangeSlider`.

### 5.3. Bubbling (всплывающая, «восходящая»)

- Событие идёт **от источника вверх** к корню визуального дерева.
- Самая частая стратегия в WPF: `Click`, `MouseDown`, `Loaded`, `SelectionChanged`.
- Удобна, когда родительский контейнер хочет знать, что произошло внутри дочерних элементов, не подписываясь на каждый дочерний отдельно.
- В нашем коде: `RangeChanged` в `PriceRangeSlider`.

**Демонстрация разницы (в `MainWindow.xaml.cs`):** при изменении цены в `PriceRangeSlider` в окно `Debug` выводится:

```
[Tunnel]  PreviewRangeChanged на самом UserControl
[Tunnel]  PreviewRangeChanged достиг Window
[Bubble]  RangeChanged на самом UserControl
[Bubble]  RangeChanged достиг Window
[Direct]  RatingChanged — это событие НЕ должно достигать Window
```

Видно, что Tunnel и Bubble доходят до Window, а Direct — нет.

---

## 6. Как создать RoutedEvent?

Создание `RoutedEvent` — в 3 шага:

1. **Регистрация** через `EventManager.RegisterRoutedEvent(...)` в статическом поле.
2. **CLR-обёртка** событийного типа (`event RoutedEventHandler`) с аксессорами `add` / `remove`, которые вызывают `AddHandler` / `RemoveHandler`.
3. **Возбуждение** события через `RaiseEvent(new RoutedEventArgs(...))`.

```csharp
public partial class RatingStars : UserControl
{
    // 1) Регистрация
    public static readonly RoutedEvent RatingChangedEvent =
        EventManager.RegisterRoutedEvent(
            "RatingChanged",                   // имя события (строка)
            RoutingStrategy.Direct,            // стратегия маршрутизации
            typeof(RoutedEventHandler),        // тип обработчика
            typeof(RatingStars));              // тип владельца

    // 2) CLR-обёртка
    public event RoutedEventHandler RatingChanged
    {
        add    => AddHandler(RatingChangedEvent, value);
        remove => RemoveHandler(RatingChangedEvent, value);
    }

    // 3) Возбуждение (внутри OnRatingChanged — PropertyChangedCallback DP)
    private static void OnRatingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RatingStars control)
        {
            control.BuildStars();
            control.RaiseEvent(new RoutedEventArgs(RatingChangedEvent));
        }
    }
}
```

**Параметры `RegisterRoutedEvent`:**

| Параметр | Назначение |
|---|---|
| `name` | Имя события (строка). Должно совпадать с именем CLR-обёртки. |
| `routingStrategy` | `Direct`, `Tunnel` или `Bubble`. |
| `handlerType` | Тип делегата обработчика — обычно `RoutedEventHandler`. |
| `ownerType` | Тип класса-владельца. |

**Подписка на событие:**

- В XAML: `<uc:PriceRangeSlider RangeChanged="PriceRange_Changed"/>`
- В коде: `control.RangeChanged += Handler;`
- Через `AddHandler` (позволяет подписываться на туннельные/всплывающие события у любого родителя, даже если у него нет CLR-обёртки):
  ```csharp
  AddHandler(PriceRangeSlider.RangeChangedEvent, (RoutedEventHandler)OnRangeChanged);
  ```

---

## 7. Поясните концепцию Command в WPF? В чём её преимущества?

**Концепция Command** — это механизм WPF, отделяющий **логику действия** от **визуального элемента**, который это действие вызывает. Вместо подписки на `Click` кнопки и написания логики прямо в обработчике, действие описывается как отдельная сущность — команда, — а UI-элемент только «вызывает» команду.

**Основные сущности:**

| Сущность | Назначение |
|---|---|
| `ICommand` | Интерфейс с методами `Execute(parameter)` и `CanExecute(parameter)` и событием `CanExecuteChanged`. |
| `RoutedCommand` | Базовый класс WPF-команды, использующий маршрутизацию событий. Не содержит логики — только имя + жест + тип владельца. |
| `RoutedUICommand` | Наследник `RoutedCommand`, добавляющий текст для UI (`Text`-свойство). |
| `CommandBinding` | Связывает команду с обработчиками `Executed` и `CanExecute` в конкретном контексте. |
| `ICommandSource` | `Button`, `MenuItem`, `InputBinding` — элементы, которые могут вызывать команду. |

**Преимущества:**

1. **Разделение логики и UI.** Команда не зависит от того, чем она вызвана — кнопкой, пунктом меню или горячей клавишей.
2. **Несколько источников одной команды.** Одна команда может вызываться из кнопки, тулбара, контекстного меню и Ctrl+S — везде будет одно действие.
3. **Автоматическое управление доступностью.** `CanExecute` автоматически включает/выключает все элементы, привязанные к команде. Не нужно вручную синхронизировать `IsEnabled` кнопок.
4. **Горячие клавиши.** `InputGestureCollection` позволяет привязать жесты (Ctrl+S, F5) к команде на уровне всего приложения.
5. **Маршрутизация.** `RoutedCommand` использует дерево элементов: можно обработать команду на уровне окна, не передавая её в каждый дочерний контрол.
6. **Тестируемость.** Команду можно вызвать из unit-теста без UI.

---

## 8. Как используются команды?

**Шаг 1.** Определить команду — либо как экземпляр `ICommand` (через `RelayCommand` в ViewModel), либо как статическое поле `RoutedUICommand`.

**Шаг 2.** Привязать UI-элемент (`Button`, `MenuItem`) к команде через свойство `Command`.

**Шаг 3.** Обеспечить выполнение команды:

- Для `RelayCommand` — логика живёт прямо в делегатах команды (`execute`, `canExecute`), в ViewModel.
- Для `RoutedUICommand` — нужно зарегистрировать `CommandBinding` где-то в визуальном дереве (обычно в `Window.CommandBindings`).

**Пример с `RelayCommand` (лаб. №6, существующий код):**

```csharp
// ViewModel
public ICommand AddToCartCommand { get; }

public MainViewModel(...)
{
    AddToCartCommand = new RelayCommand(
        p => AddToCart(p as Product),                           // execute
        p => p is Product product && !product.IsOutOfStock);    // canExecute
}
```

```xml
<!-- XAML -->
<Button Content="В корзину"
        Command="{Binding AddToCartCommand}"
        CommandParameter="{Binding}"/>
```

**Пример с `RoutedUICommand` (лаб. №7, новый код):**

```xml
<!-- Window.CommandBindings -->
<Window.CommandBindings>
    <CommandBinding Command="{x:Static helpers:AppCommands.SaveCartCommand}"
                    Executed="SaveCart_Executed"
                    CanExecute="SaveCart_CanExecute"/>
</Window.CommandBindings>

<!-- Кнопка вызывает команду -->
<Button Content="Сохранить"
        Command="{x:Static helpers:AppCommands.SaveCartCommand}"/>
```

```csharp
// Code-behind окна
private void SaveCart_Executed(object sender, ExecutedRoutedEventArgs e)
{
    if (DataContext is MainViewModel vm) vm.SaveCart();
}

private void SaveCart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
{
    e.CanExecute = DataContext is MainViewModel vm && vm.CartItems.Any();
}
```

Также команда автоматически становится доступной через горячую клавишу `Ctrl+S` (если жест задан в `InputGestureCollection`).

---

## 9. Как создать RoutedUICommand?

`RoutedUICommand` — это готовый класс WPF, наследник `RoutedCommand`, добавляющий текстовое описание для UI. Создаётся как статическое поле в отдельном статическом классе-контейнере.

```csharp
using System.Windows.Input;

namespace SportNutritionShop.Helpers
{
    /// <summary>
    /// Пользовательские команды на основе RoutedUICommand.
    /// </summary>
    public static class AppCommands
    {
        public static readonly RoutedUICommand SaveCartCommand =
            new RoutedUICommand(
                "Сохранить корзину",      // Text — описание для UI
                "SaveCart",                // Name — уникальное имя команды
                typeof(AppCommands),       // OwnerType — тип владельца
                new InputGestureCollection
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)   // жест Ctrl+S
                });
    }
}
```

**Параметры конструктора:**

| Параметр | Назначение |
|---|---|
| `text` | Человекочитаемое описание. Используется в `MenuItem` без явного `Header`, в подсказках. |
| `name` | Уникальное имя команды. Используется при диспетчеризации. |
| `ownerType` | Тип, к которому привязана команда. Нужен для системы регистрации. |
| `inputGestures` | Коллекция горячих клавиш (`KeyGesture`), которые автоматически вызывают команду. |

**Использование в XAML:**

```xml
<!-- 1) Привязать обработчики -->
<Window.CommandBindings>
    <CommandBinding Command="{x:Static helpers:AppCommands.SaveCartCommand}"
                    Executed="SaveCart_Executed"
                    CanExecute="SaveCart_CanExecute"/>
</Window.CommandBindings>

<!-- 2) Вызывать из кнопки -->
<Button Content="Сохранить"
        Command="{x:Static helpers:AppCommands.SaveCartCommand}"/>
```

**Использование в коде (C#):**

```csharp
// Подписка на выполнение
CommandBindings.Add(new CommandBinding(
    AppCommands.SaveCartCommand,
    SaveCart_Executed,
    SaveCart_CanExecute));

// Программный вызов команды
AppCommands.SaveCartCommand.Execute(null, this);
```

**Привязка жеста через InputBinding (альтернативный способ):**

```xml
<Window.InputBindings>
    <KeyBinding Command="{x:Static helpers:AppCommands.SaveCartCommand}"
                Key="S" Modifiers="Control"/>
</Window.InputBindings>
```

В этом случае жест уже задан в `InputGestureCollection`, и `KeyBinding` redundant — но иногда удобно задавать жесты именно в XAML на уровне окна, а не в классе команды.
