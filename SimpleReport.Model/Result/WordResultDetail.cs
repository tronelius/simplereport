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
    public class WordResultDetail : WordResultBase
    {
        public WordResultDetail()
        {
            
        }
        public WordResultDetail(Report report, Template template) :base(report, template) { }

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

        public override ResultInfo ResultInfo { get { return new ResultInfo("WordResultDetail", "Word one row"); } }
        
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
                    foreach (DataColumn col in table.Rows[0].Table.Columns)
                    {
                        fieldContentsList.Add(new FieldContent(col.ColumnName, _replacer.Replace(table.Rows[0][col.ColumnName].ToString())));
                    }
                    var content = new Content(fieldContentsList.ToArray());
                    FillWordTemplate(doc, content, false);
                    doc.MainDocumentPart.Document.Save();
                    sources.Add(new Source(new WmlDocument(new OpenXmlPowerToolsDocument(ms.ToArray()))));
                }
            }

        }

    }

}