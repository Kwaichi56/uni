# Лабораторная работа: Service Locator на C# (.NET 10.0)

## Состав
- `ServiceLocator` — библиотека с реализацией паттерна Service Locator.
- `Test_ServiceLocator` — консольное приложение для демонстрации `Transient`, `Singleton`, `Scoped`.

## Как открыть
1. Создайте пустой solution в Visual Studio или Rider.
2. Добавьте в него оба проекта из архива.
3. Назначьте `Test_ServiceLocator` запускаемым проектом.
4. Запустите приложение.

## Что демонстрируется
- `Transient` — каждый раз создаётся новый объект.
- `Singleton` — возвращается один и тот же объект.
- `Scoped` — один объект внутри одного scope и разные объекты в разных scope.
