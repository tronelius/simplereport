using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using SimpleReport.Model.Extensions;
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
            RenderTableReport(tables, sources);
            return MergeSources(sources);
        }


        public override ResultInfo ResultInfo { get { return new ResultInfo("WordResultPlain", "Word listing"); } }
        
        private void RenderTableReport(List<DataTable> tables, List<Source> sources)
        {
            using (var tdata = new MemoryStream(TemplateData))
            using (var ms = new MemoryStream())
            {
                tdata.CopyTo(ms);
                ms.Position = 0;
                using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, true))
                {
                    List<IContentItem> fieldContentsList = new List<IContentItem>();
                    var tableContentControls = FindTableContentControlsInTemplate(doc).ToList();
                    var nonTableContentControls = FindNonTableContentControlsInTemplate(doc).ToList();

                    int index = 0;
                    foreach (DataTable dataTable in tables)
                    {
                        if (nonTableContentControls.Count > 0 && index == 0 && dataTable.Rows.Count==1) //Content controls outside of tables exists, first table has only one row of data, match these
                        {
                            var firstrow = dataTable.Rows[0];
                            foreach (DataColumn column in firstrow.Table.Columns)
                            {
                                var foundControl = nonTableContentControls.FirstOrDefault(c => c.SdtTagName().ToLower().Equals(column.ColumnName.ToLower()));
                                if (foundControl != null)
                                    fieldContentsList.Add(new FieldContent(foundControl.SdtTagName(), _replacer.Replace(firstrow[column.ColumnName].ToString())));
                            }

                            if (fieldContentsList.Count > 0) //First table has only one row and data matches contentControls, assume it should be excluded.
                                continue;
                        }

                        if (tableContentControls.Count() >= index+1) {  //check if tables exist in word-template, Match DataTable in SQL-result to table in word-template 1-1
                            string name = tableContentControls[index].SdtTagName();
                            TableContent tableContent = new TableContent(name);
                            var contentControlsInsideTable = FindContentControlsInsideElement(tableContentControls[index]);
                            foreach (DataRow row in dataTable.Rows)
                            {
                                List<FieldContent> fieldContentTableList = new List<FieldContent>();
                                foreach (DataColumn col in row.Table.Columns)
                                {
                                    var foundControl = contentControlsInsideTable.FirstOrDefault(c => c.SdtTagName().ToLower().Equals(col.ColumnName.ToLower()));
                                    if (foundControl != null)
                                        fieldContentTableList.Add(new FieldContent(foundControl.SdtTagName(), _replacer.Replace(row[col.ColumnName].ToString())));
                                }
                                if (fieldContentTableList.Count > 0)
                                    tableContent.AddRow(fieldContentTableList.ToArray());
                            }
                            if (tableContent.Rows?.Count > 0)
                                fieldContentsList.Add(tableContent);
                        }
                        index++;
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