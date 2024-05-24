using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebSearchApp
{
    public static class HtmlUtilities
    {
        public static string ConvertToPlainText(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.InnerText;
        }

        public static string GetTitle(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//title");
            return titleNode != null ? titleNode.InnerText : "No Title";
        }
    }
}