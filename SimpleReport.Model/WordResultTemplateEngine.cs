using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using SimpleReport.Model.Helpers;
using TemplateEngine.Docx;
using Source = OpenXmlPowerTools.Source;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;

namespace SimpleReport.Model
{
    public class WordResultTemplateEngine : Result
    {
        private List<DataTable> Tables { get; set; }
        private List<List<dynamic>> Dynamic { get; set; }
        public WordResultTemplateEngine(List<DataTable> tables, Report report, byte[] templateData) : base(tables.First(), report, templateData)
        {
            Tables = tables;
        }

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
            var lastRow = Table.Rows[Table.Rows.Count - 1];

            DataColumn[] columns = new DataColumn[Table.Columns.Count];
            Table.Columns.CopyTo(columns, 0);
            var columnNames = columns.Select(a => a.ColumnName.ToLower()).ToList();
            var moreTablesThanOne = Tables.Count > 1 && Table.Columns.Contains(FieldHandles.Merge) && Tables[1].Columns.Contains(FieldHandles.Merge);
            ILookup<object, DataRow> grouped = null;
            foreach (DataTable dataTable in Tables.Skip(1).Take(1)) //second table hardcoded for now
            {
                grouped = dataTable.AsEnumerable().ToLookup(row => row[FieldHandles.Merge]);
            }

            foreach (DataRow row in Table.Rows)
            {
                using (var tdata = new MemoryStream(TemplateData))
                using (var ms = new MemoryStream())
                {
                    tdata.CopyTo(ms); //we need a new template for every row.
                    ms.Position = 0;
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, true))
                    {
                        //Table fields
                        TableContent tableContent = new TableContent(FieldHandles.table);
                        List<IContentItem> FieldContentsList = new List<IContentItem>();
                        if (moreTablesThanOne)
                        {
                            List<FieldContent> FieldContentTableList = new List<FieldContent>();

                            var mergeValueForRow = row[FieldHandles.Merge];
                            foreach (DataRow row2 in grouped[mergeValueForRow])
                            {
                                FieldContentTableList = new List<FieldContent>();
                                foreach (DataColumn col in row2.Table.Columns)
                                {
                                    if (col.ColumnName.ToLower() != FieldHandles.Merge)
                                        FieldContentTableList.Add(new FieldContent(col.ColumnName, row2[col.ColumnName].ToString()));
                                }
                                tableContent.AddRow(FieldContentTableList.ToArray());
                            }
                        }
                        FieldContentsList.Add(tableContent);

                        //Normal fields
                        foreach (string columnName in columnNames)
                        {
                            if (columnName.ToLower() != FieldHandles.Merge)
                                FieldContentsList.Add(new FieldContent(columnName, row[columnName].ToString()));
                        }

                        var content = new Content(FieldContentsList.ToArray());

                        XDocument document = GetXDocument(doc);
                        using (var outputDocument = new TemplateProcessor(document).SetRemoveContentControls(true)
                            //.SetNoticeAboutErrors(false)
                            )
                        {
                            outputDocument.FillContent(content);
                            using (var xw = XmlWriter.Create(doc.MainDocumentPart.GetStream(FileMode.Create, FileAccess.Write)))
                            {
                                document.Save(xw);
                            }
                        }

                        if (row != lastRow)
                            AddPageBreak(doc);

                        doc.MainDocumentPart.Document.Save();
                        sources.Add(new Source(new WmlDocument(new OpenXmlPowerToolsDocument(ms.ToArray()))));
                    }
                }
            }

            return MergeSources(sources);
        }

        private static XDocument GetXDocument(WordprocessingDocument myDoc)
        {
            // Load the main document part into an XDocument
            XDocument mainDocumentXDoc;
            using (Stream str = myDoc.MainDocumentPart.GetStream())
            using (XmlReader xr = XmlReader.Create(str))
                mainDocumentXDoc = XDocument.Load(xr);
            return mainDocumentXDoc;
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
    }

}