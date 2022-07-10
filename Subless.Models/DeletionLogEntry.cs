using System;

namespace Subless.Models
{
    public class DeletionLogEntry
    {
        public string Role { get; set; }
        public DateTimeOffset DeleteDate { get; set; }
        public string Id { get; set; }
    }
}
