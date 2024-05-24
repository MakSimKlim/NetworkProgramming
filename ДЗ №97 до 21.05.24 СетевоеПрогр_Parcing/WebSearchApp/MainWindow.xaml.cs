using HtmlLibrary;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WebSearchApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string url = UrlTextBox.Text.Trim();

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                try
                {
                    string html = await HtmlFetcher.GetHtmlAsync(url);
                    string plainText = HtmlUtilities.ConvertToPlainText(html);
                    string title = HtmlUtilities.GetTitle(html);

                    ResultWindow resultWindow = new ResultWindow(title, plainText);
                    resultWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке страницы: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Введите правильный URL.");
            }
        }
    }
}
