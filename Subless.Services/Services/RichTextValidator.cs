using Ganss.Xss;
using HtmlAgilityPack;
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
                throw new AccessViolationException($"Problem with message {HttpUtility.UrlEncode(message)} ");
            }
            return Sanitize(message);
        }

        private static string[] BannedCharacters = new[] { ";", "[", "]", "%", "javascript", "\\\\", "-script", "(", ")", "\\" };
        private static string[] WhitelisedLinks = new[] {"https://www.patreon.com",
        "https://www.paypal.com",
        "https://www.subscribestar.com",
        "https://ko-fi.com",
        "https://twitter.com",
        "https://www.hentai-foundry.com",
        "https://linktr.ee"};

        private static bool MessageValid(string message)
        {
            if (message.Length > 1000) { return false; }
            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(message);

            if (doc.DocumentNode.Descendants()
                            .Any(n => n.Name == "script" || n.Name == "style"))
            {
                return false;
            }
            if (!LinksInWhiteList(doc))
            {
                return false;
            }

            return true;
        }

        private static bool LinksInWhiteList(HtmlDocument doc)
        {
            var linkElements = doc.DocumentNode.Descendants().Where(x => x.Name == "a");
            foreach (var linkElement in linkElements)
            {
                if (linkElement.Attributes.Where(x => x.Name == "href").Any(linkAttribute => !WhitelisedLinks.Any(whiteListed => linkAttribute.Value.StartsWith(whiteListed))))
                {
                    return false;
                }
            }
            return true;
        }

        private static string Sanitize(string message)
        {
            foreach (string c in BannedCharacters)
            {
                message = message.Replace(c, string.Empty);
            }
            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(message);
        }
    }
}
