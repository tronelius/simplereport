using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Common.Model
{
    public class UpdateTemplateText
    {
        public Guid ReportGuid { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }
}
