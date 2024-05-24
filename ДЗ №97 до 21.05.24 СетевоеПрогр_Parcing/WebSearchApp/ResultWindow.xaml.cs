using System.Windows;

namespace WebSearchApp
{
    public partial class ResultWindow : Window
    {
        public ResultWindow(string title, string content)
        {
            InitializeComponent();
            this.Title = title;
            ContentTextBlock.Text = content;
        }
    }
}
