using Subless.Models;
using System.Collections.Generic;

namespace Subless.Services.Services
{
    public interface IEmailService
    {
        string GetEmailBody(List<Payment> payments);
    }
}