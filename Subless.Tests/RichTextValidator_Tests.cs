using Moq;
using Subless.Data;
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
        [Theory]
        [FileData("xssTestData.txt")]

        public void RichTextValidator_WithXss_ThrowsExceptionOrEscapes(string input)
        {
            try
            {
                var result = RichTextValidator.SanitizeInput(input);
                Assert.NotEqual(input, result);
            }
            catch (AccessViolationException ex)
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
