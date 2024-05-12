using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Введите URL веб-страницы:");
        string url = Console.ReadLine();

        try
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(responseBody);

            // Получение заголовка страницы
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
            string title = titleNode != null ? titleNode.InnerText : "Заголовок не найден";

            // Подсчёт тегов
            int tagsCount = htmlDoc.DocumentNode.Descendants().Where(n => n.NodeType == HtmlNodeType.Element).Count();

            Console.WriteLine($"Заголовок страницы: {title}");
            Console.WriteLine($"Количество HTML-тегов (не включая закрывающие): {tagsCount}");
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }
}
