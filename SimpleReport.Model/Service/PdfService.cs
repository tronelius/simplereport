using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Aspose.Words;
using SimpleReport.Model.Helpers;
using SimpleReport.Model.Result;

namespace SimpleReport.Model.Service
{
    public interface IPdfService
    {
        ResultFileInfo ConvertToPdf(ResultFileInfo result);
    }

    public class PdfService : IPdfService
    {
        public ResultFileInfo ConvertToPdf(ResultFileInfo result)
        {
            if (result == null)
                return null;

            using (var stream = new MemoryStream(result.Data))
            {
                var doc = new Document(stream);

                var pdfStream = new MemoryStream();
                doc.Save(pdfStream, SaveFormat.Pdf);

                var data = pdfStream.ToArray();

                var ext = Path.GetExtension(result.FileName);
                var name = result.FileName.Replace(ext, ".pdf");

                //var name = Path.GetFileNameWithoutExtension(result.FileName) + ".pdf"; //NOTE: this does not work since we have a stupid filename that .net cannot parse.

                return new ResultFileInfo(name, MimeTypeHelper.PdfMime, data);
            }
        }
    }
}
