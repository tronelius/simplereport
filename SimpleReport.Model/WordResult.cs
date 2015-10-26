using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using SimpleReport.Model.Helpers;
using Source = OpenXmlPowerTools.Source;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace SimpleReport.Model
{
    public class WordResult : Result
    {
        private List<DataTable> Tables { get; set; } 

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

                            if (reportBookmark.Type == "TBL")//tables are special
                            {
                                HandleTable(reportBookmark, bookmarkStart, row);
                            }
                            else if (Table.Columns.Contains(reportBookmark.Name))
                            {
                                HandleText(row, reportBookmark, bookmarkStart);
                            }
                        }

                        AddPageBreak(doc);

                        doc.MainDocumentPart.Document.Save();
                        sources.Add(new Source(new WmlDocument(new OpenXmlPowerToolsDocument(ms.ToArray()))));
                    }
                }
            }
            
            return MergeSources(sources);
        }

        private static byte[] MergeSources(List<Source> sources)
        {
            var merged = DocumentBuilder.BuildDocument(sources);
            var path = Path.GetTempFileName();
            merged.SaveAs(path);

            var bytes = File.ReadAllBytes(path);
            File.Delete(path);
            return bytes;
        }

        private static void AddPageBreak(WordprocessingDocument doc)
        {
            var pageBreak = new Paragraph(new Run(new Break() {Type = BreakValues.Page}));
            doc.MainDocumentPart.Document.Descendants<Paragraph>().Last().InsertAfterSelf(pageBreak);
        }

        private static void HandleText(DataRow row, ReportBookmark reportBookmark, BookmarkStart bookmarkStart)
        {
            var value = row[reportBookmark.Name];
            var runElement = GetRunElementForValue(value);

            bookmarkStart.InsertAfterSelf(runElement);
        }

        private void HandleTable(ReportBookmark reportBookmark, BookmarkStart bookmarkStart, DataRow row)
        {
            var dataTable = Tables[reportBookmark.Number];
            var wordTable = bookmarkStart.Parent.NextSibling<Table>();
            var firstTableRow = wordTable.GetFirstChild<TableRow>();
            var cellCount = firstTableRow.Descendants<TableCell>().Count();

            foreach (DataRow subRow in dataTable.Rows)
            {
                if (!subRow["merge_id"].ToString().Equals(row["merge_id"]))//merge_id is currently the magic string that we use to merge.
                    continue;

                TableRow rowCopy = new TableRow();

                for (int i = 0; i < cellCount; i++)
                {
                    var cell = new TableCell();
                    var runElement = GetRunElementForValue(subRow[i]);
                    cell.AppendChild(new Paragraph(runElement));
                    rowCopy.AppendChild(cell);
                }

                firstTableRow.InsertAfterSelf(rowCopy);
            }
        }

        private static Run GetRunElementForValue(object value)
        {
            //TODO: add formatting of values? like dates.. already use that in one of the controllers. reuse?
            var textElement = new Text(value.ToString());
            var runElement = new Run(textElement);
            return runElement;
        }

        public WordResult(List<DataTable> tables, Report report, byte[] templateData) : base(tables.First(), report, templateData)
        {
            Tables = tables;
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