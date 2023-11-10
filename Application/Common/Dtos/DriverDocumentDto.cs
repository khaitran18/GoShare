using Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class DriverDocumentDto
    {
        public DocumentTypeEnumerations Type { get; set; }
        public string Url { get; set; } = null!;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
