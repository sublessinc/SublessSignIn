using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subless.Data
{
    public class User
    {
        public Guid Id { get; set; }
        public string CognitoId { get; set; }
        public string StripeId { get; set; }
    }
}
