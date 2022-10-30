using Moq;
using Subless.Data;
using Subless.Models;
using Subless.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Subless.Tests
{
    public class RichTextValidator_Tests
    {
        public static List<string> WhitelisedLinks = new List<string> {
            "https://www.patreon.com",
            "https://www.paypal.com",
            "https://www.subscribestar.com",
            "https://ko-fi.com",
            "https://twitter.com",
            "https://www.hentai-foundry.com",
            "https://linktr.ee",
            "https://*fanbox.cc"
        };
        [Theory]
        [FileData("xssTestData.txt")]

        public void RichTextValidator_WithXss_ThrowsExceptionOrEscapes(string input)
        {
            try
            {
                var result = RichTextValidator.SanitizeInput(input, WhitelisedLinks);
                Assert.NotEqual(input, result);
            }
            catch (NotSupportedException ex)
            {
                Assert.True(true, "Exception not thrown on xss input");
            }
            catch (InputInvalidException ex)
            {
                Assert.True(true, "Exception not thrown on xss input");
            }
        }

        [Theory]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://www.paypal.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://www.patreon.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://twitter.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://linktr.ee/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://www.subscribestar.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://ko-fi.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for donating! Check out <a href=\"https://www.patreon.com/user?u=3342350\">my patreon</a> for \" co</p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://coolguy.fanbox.cc/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://fanbox.cc/user?u=3342350\">My patreon</a></p>")]
        [InlineData("Thanks for Donating to Jon!")]
        public void RichTextValidator_WithValidInput_PreservesInput(string input)
        {
            var result = RichTextValidator.SanitizeInput(input, WhitelisedLinks);
            Assert.Equal(input, result);
        }

        [Theory]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://www.notpatreon.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://fake.patreon.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://www.pareon.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://www.patreon.org/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://ww.patreon.com/user?u=3342350\">My patreon</a></p>")]
        [InlineData("<p>Thanks for Donating to Jon! <a href=\"https://coolguy/fanbox.cc/user?u=3342350\">My patreon</a></p>")]
        public void RichTextValidator_WithInvalidLink_ThrowsError(string input)
        {
            try
            {
                var result = RichTextValidator.SanitizeInput(input, WhitelisedLinks);
                Assert.NotEqual(input, result);
            }
            catch (NotSupportedException ex)
            {
                Assert.True(true, "Exception not thrown on xss input");
            }
            catch (InputInvalidException ex)
            {
                Assert.True(true, "Exception not thrown on xss input");
            }
        }
    }


    public class FileDataAttribute : DataAttribute
    {
        private readonly string _filePath;

        /// <summary>
        /// Load data from a JSON file as the data source for a theory
        /// </summary>
        /// <param name="filePath">The absolute or relative path to the JSON file to load</param>
        public FileDataAttribute(string filePath)
        {
            _filePath = filePath;
        }

        /// <inheritDoc />
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) { throw new ArgumentNullException(nameof(testMethod)); }

            // Get the absolute path to the JSON file
            var path = Path.IsPathRooted(_filePath)
                ? _filePath
                : Path.GetRelativePath(Directory.GetCurrentDirectory(), _filePath);

            if (!File.Exists(path))
            {
                throw new ArgumentException($"Could not find file at path: {path}");
            }
            
            // Load the file
            return File.ReadAllLines(_filePath).Select(x => new[] { x });
        }
    }
}
