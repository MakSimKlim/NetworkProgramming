using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using HtmlAgilityPack;

namespace ДЗ__96_до_18._05._24_СетевоеПрогр_HttpЗапросы
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlTextBox.Text.Trim();
            string searchText = SearchTextBox.Text.Trim();

            ResultsListBox.Items.Clear(); // Очищаем результаты предыдущего поиска

            SearchTextOnPage(url, searchText);

            MessageBox.Show("Поиск завершен!");
        }

        private void SearchTextOnPage(string url, string searchText)
        {
            try
            {
                WebClient client = new WebClient();
                string html = client.DownloadString(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                string text = doc.DocumentNode.InnerText;

                if (text.Contains(searchText))
                {
                    int index = text.IndexOf(searchText);
                    string snippet = text.Substring(index, Math.Min(50, text.Length - index));
                    SaveResultToFile(url, snippet, text);
                    ResultsListBox.Items.Add($"URL: {url} - {snippet}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске на странице {url}: {ex.Message}");
            }
        }

        private void SaveResultToFile(string url, string snippet, string content)
        {
            string result = $"URL: {url}\nSnippet: {snippet}\nContent:\n{content}\n\n";

            try
            {
                string filePath = "search_results.txt";
                File.AppendAllText(filePath, result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении результата в файл: {ex.Message}");
            }
        }
    }
}
