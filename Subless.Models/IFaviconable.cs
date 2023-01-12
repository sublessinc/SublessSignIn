using System;

namespace Subless.Models
{
    public interface IFaviconable
    {
        Uri Favicon { get; set; }
        Guid PartnerId { get; set; }
    }
}