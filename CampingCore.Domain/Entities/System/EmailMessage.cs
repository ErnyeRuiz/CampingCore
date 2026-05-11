using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.Domain.Entities.System
{
    public record EmailMessage(
        string ToEmail,
        string ToName,
        string Subject,
        string HtmlContent
    );
}
