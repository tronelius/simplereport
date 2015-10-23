using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleReport.Helpers
{
    public static class MimeTypeHelper
    {
        public static bool IsWord(string mime)
        {
            return mime == "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        }
    }
}