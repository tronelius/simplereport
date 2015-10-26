using System.Data;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SimpleReport.Model.Helpers;

namespace SimpleReport.Model
{
    public class WordResult : Result
    {
        protected override string getMimeType()
        {
            return MimeTypeHelper.WordMime;
        }

        protected override string getFileExtension()
        {
            return ".docx";
        }
        
        public override byte[] AsFile()
        {

            using (var ms = new MemoryStream(TemplateData))
            using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, true))
            {
                foreach (BookmarkStart bookmarkStart in doc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                {
                    var textElement = new Text("BOOKMARK");
                    var runElement = new Run(textElement);

                    bookmarkStart.InsertAfterSelf(runElement);
                }

                doc.MainDocumentPart.Document.Save();
                return ms.ToArray();
            }
        }

        public WordResult(DataTable table, Report report, byte[] templateData) : base(table, report, templateData)
        {
        }

        public WordResult(IDataReader dataReader, Report report) : base(dataReader, report)
        {
        }
    }
}