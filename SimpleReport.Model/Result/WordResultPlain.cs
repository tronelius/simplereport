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
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Source = OpenXmlPowerTools.Source;

namespace SimpleReport.Model.Result
{
    public class WordResultPlain : WordResultBase
    {
        public WordResultPlain()
        {
            
        }
        public WordResultPlain(Report report, Template template) :base(report, template) { }

        public override ResultFileInfo Render(List<DataTable> tables)
        {
            if (TemplateData == null) //dependent on template to function
                return null;

            var sources = new List<Source>();
            var table = tables.First();
            var lastRow = table.Rows[table.Rows.Count - 1];
            DataColumn[] columns = new DataColumn[table.Columns.Count];
            table.Columns.CopyTo(columns, 0);
            var columnNames = columns.Select(a => a.ColumnName.ToLower()).ToList();
            RenderSimpleTableReport(table, columnNames, sources);
            return MergeSources(sources);
        }


        public override ResultInfo ResultInfo { get { return new ResultInfo("WordResultPlain", "Word"); } }
        
        private void RenderSimpleTableReport(DataTable table, List<string> columnNames, List<Source> sources)
        {
            using (var tdata = new MemoryStream(TemplateData))
            using (var ms = new MemoryStream())
            {
                tdata.CopyTo(ms);
                ms.Position = 0;
                using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, true))
                {
                    List<IContentItem> fieldContentsList = new List<IContentItem>();
                    TableContent tableContent = new TableContent(FieldHandles.table);
                        foreach (DataRow row in table.Rows)
                        {
                            List<FieldContent> fieldContentTableList = new List<FieldContent>();
                            foreach (DataColumn col in row.Table.Columns)
                            {
                                fieldContentTableList.Add(new FieldContent(col.ColumnName, _replacer.Replace(row[col.ColumnName].ToString())));
                            }
                            tableContent.AddRow(fieldContentTableList.ToArray());
                        }
                        fieldContentsList.Add(tableContent);
                    var content = new Content(fieldContentsList.ToArray());
                    FillWordTemplate(doc, content, false);
                    doc.MainDocumentPart.Document.Save();
                    sources.Add(new Source(new WmlDocument(new OpenXmlPowerToolsDocument(ms.ToArray()))));
                }
            }

        }

    }

}