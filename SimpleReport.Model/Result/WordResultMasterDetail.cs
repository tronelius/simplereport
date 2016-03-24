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
    public class WordResultMasterDetail : WordResultBase
    {
        public WordResultMasterDetail()
        {
            
        }
        public WordResultMasterDetail(Report report, Template template) : base(report, template)
        {
            //Tables = tables;
        }

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
            //bool masterDetailDetected = Tables.Count > 1 && Table.Columns.Contains(FieldHandles.Merge) && Tables[1].Columns.Contains(FieldHandles.Merge);
            ILookup<object, DataRow> masterDetailData = null;
            masterDetailData = tables.Skip(1).Take(1).First().AsEnumerable().ToLookup(row => row[FieldHandles.Merge]);
            RenderMasterDetailReport(tables, masterDetailData, columnNames, lastRow, sources);
            return MergeSources(sources);
        }

        public override ResultInfo ResultInfo { get { return new ResultInfo("WordResultMasterDetail", "Word Master/Detail"); } }

        private void RenderMasterDetailReport(List<DataTable> detailData, ILookup<object, DataRow> masterDetailData, List<string> columnNames, DataRow lastRow, List<Source> sources)
        {
            var table = detailData.First();
            foreach (DataRow row in table.Rows)
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
                        if (masterDetailData != null)
                        {
                            List<FieldContent> FieldContentTableList = new List<FieldContent>();

                            var mergeValueForRow = row[FieldHandles.Merge];
                            foreach (DataRow row2 in masterDetailData[mergeValueForRow])
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

                        FillWordTemplate(doc, content, lastRow != row);

                        doc.MainDocumentPart.Document.Save();
                        sources.Add(new Source(new WmlDocument(new OpenXmlPowerToolsDocument(ms.ToArray()))));
                    }
                }
            }
        }



    }

}