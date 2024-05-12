using System; // Подключение базовой библиотеки .NET
using System.Net.Http; // Подключение библиотеки для работы с HTTP
using System.Windows; // Подключение библиотеки для работы с WPF окнами
using HtmlAgilityPack; // Подключение библиотеки для работы с HTML

namespace HttpClientApp // Изменено название пространства имен для избежания конфликта
{
    public partial class MainWindow : Window // Объявление класса MainWindow, наследуемого от Window
    {
        public MainWindow() // Конструктор класса MainWindow
        {
            InitializeComponent(); // Метод для инициализации компонентов интерфейса, определен в XAML
        }

        private async void FetchButton_Click(object sender, RoutedEventArgs e) // Асинхронный обработчик события клика на кнопку
        {
            string url = UrlTextBox.Text; // Считывание URL из текстового поля
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient()) // Создание экземпляра HttpClient
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url); // Отправка асинхронного HTTP GET запроса
                    response.EnsureSuccessStatusCode(); // Проверка статуса ответа на корректность
                    string content = await response.Content.ReadAsStringAsync(); // Чтение содержимого ответа как строки

                    HtmlDocument doc = new HtmlDocument(); // Создание экземпляра HtmlDocument
                    doc.LoadHtml(content); // Загрузка HTML-контента в документ

                    // Получение заголовка страницы из HTML
                    var titleNode = doc.DocumentNode.SelectSingleNode("//title");
                    var pTags = doc.DocumentNode.SelectNodes("//p"); // Получение всех тегов <p>
                    var firstPTag = pTags?.Count > 0 ? pTags[0].InnerText : "No <p> tags found"; // Получение текста первого тега <p>

                    // Вывод информации в текстовый блок
                    OutputTextBlock.Text = $"Title: {titleNode?.InnerText ?? "No Title"}\n" +
                                           $"Count of <p> tags: {pTags?.Count.ToString() ?? "0"}\n" +
                                           $"First paragraph: {firstPTag}";
                }
                catch (Exception ex) // Обработка возможных исключений
                {
                    OutputTextBlock.Text = $"Error: {ex.Message}"; // Вывод сообщения об ошибке
                }
            }
        }
    }
}
