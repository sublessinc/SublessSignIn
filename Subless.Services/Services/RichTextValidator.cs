using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Subless.Services.Services
{
    public static class RichTextValidator
    {
        public static string SanitizeInput(string message)
        {
            if (!MessageValid(message))
            {
                throw new AccessViolationException($"XSS detected in input {HttpUtility.UrlEncode(message)} ");
            }
            return Sanitize(message);
        }

        private static bool MessageValid(string message)
        {
            if (message.Length > 1000) { return false; }
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(message);

            if (doc.DocumentNode.Descendants()
                            .Any(n => n.Name == "script" || n.Name == "style"))
            {
                return false;
            }
            return true;
        }

        private static string Sanitize(string message)
        {
            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(message);
        }
    }
}
