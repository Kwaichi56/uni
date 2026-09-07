namespace ASPA001
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Инициализация строителя приложения с конфигурацией и провайдерами по умолчанию
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpLogging(options => { });

            // Сборка веб-приложения на основе заданных параметров
            var app = builder.Build();

            app.UseHttpLogging();

            // Настройка маршрутизации: возврат текста при GET-запросе к корню сайта ("/")
            app.MapGet("/", () => "Мое первое ASPA");

            // Запуск приложения и начало прослушивания входящих HTTP-запросов сервером
            app.Run();
        }
    }
}