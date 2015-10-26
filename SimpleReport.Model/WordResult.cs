using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
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
            var sources = new List<Source>();

            foreach (DataRow row in Table.Rows)
            {
                using (var tdata = new MemoryStream(TemplateData))
                using (var ms = new MemoryStream())
                {
                    tdata.CopyTo(ms); //we need a new template for every row.
                    ms.Position = 0;

                    using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, true))
                    {
                        foreach (BookmarkStart bookmarkStart in doc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                        {
                            var reportBookmark = new ReportBookmark(bookmarkStart.Name);

                            if (Table.Columns.Contains(reportBookmark.Name))
                            {
                                var value = row[reportBookmark.Name];

                                //TODO: add formatting of values? like dates.. already use that in one of the controllers. reuse?
                                var textElement = new Text(value.ToString());
                                var runElement = new Run(textElement);

                                bookmarkStart.InsertAfterSelf(runElement);
                            }
                        }

                        var pageBreak = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
                        doc.MainDocumentPart.Document.Descendants<Paragraph>().Last().InsertAfterSelf(pageBreak);

                        doc.MainDocumentPart.Document.Save();
                        sources.Add(new Source(new WmlDocument(new OpenXmlPowerToolsDocument(ms.ToArray()))));
                    }
                }
            }
            
            var merged = DocumentBuilder.BuildDocument(sources);
            var path = Path.GetTempFileName();
            merged.SaveAs(path);

            var bytes = File.ReadAllBytes(path);
            File.Delete(path);

            return bytes;
        }

        public WordResult(DataTable table, Report report, byte[] templateData) : base(table, report, templateData)
        {
        }

        public WordResult(IDataReader dataReader, Report report) : base(dataReader, report)
        {
        }
    }

    public class ReportBookmark
    {
        public ReportBookmark(string bookmarkName)
        {
            var list = bookmarkName.Split('_');

            if (list.Length == 1)
            {
                Name = list[0];
            }
            else if (list.Length == 2)
            {
                Type = list[0];
                Name = list[1];
            }
            else if (list.Length == 3)
            {
                Type = list[0];
                Name = list[1];
                Number = int.Parse(list[2]);
            }
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }
}