using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OpenXmlPowerTools;
using SimpleReport.Model.Extensions;
using SimpleReport.Model.Helpers;
using SimpleReport.Model.Replacers;
using TemplateEngine.Docx;
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;

namespace SimpleReport.Model.Result
{
    public abstract class WordResultBase : Result
    {
        protected readonly IXmlReplacer _replacer = new XmlReplacer();

        public WordResultBase()
        {
        }

        protected override string getMimeType()
        {
            return MimeTypeHelper.WordMime;
        }

        protected override string getFileExtension()
        {
            return ".docx";
        }

        public WordResultBase(Report report, Template template) : base(report, template) { }

        protected IEnumerable<XElement> FindNonTableContentControlsInTemplate(WordprocessingDocument doc)
        {
            XDocument document = GetXDocument(doc);
            XElement content = document.Root.Element(W.body);
            var allDescendantsAndSelf = content.DescendantsAndSelf(W.sdt).ToList();
            return allDescendantsAndSelf.Where(d => !(d.Ancestors(W.sdt).Intersect(allDescendantsAndSelf)).Any() && !d.SdtTagName().ToLower().StartsWith("table"));
        }

        protected IEnumerable<XElement> FindTableContentControlsInTemplate(WordprocessingDocument doc)
        {
            XDocument document = GetXDocument(doc);
            XElement content = document.Root.Element(W.body);
            var allDescendantsAndSelf = content.DescendantsAndSelf(W.sdt).ToList();
            return allDescendantsAndSelf.Where(d => (d.Descendants(W.sdt).Intersect(allDescendantsAndSelf)).Any() && d.SdtTagName().ToLower().StartsWith("table"));
        }
        protected IEnumerable<XElement> FindContentControlsInsideElement(XElement element)
        {
            return element.Descendants(W.sdt).ToList();
        }

        protected void FillWordTemplate(WordprocessingDocument doc, Content content, bool addpagebreak)
        {
            XDocument document = GetXDocument(doc);
            using (var outputDocument = new TemplateProcessor(document).SetRemoveContentControls(true)) //.SetNoticeAboutErrors(false)
            {
                outputDocument.FillContent(content);
                using (var xw = XmlWriter.Create(doc.MainDocumentPart.GetStream(FileMode.Create, FileAccess.Write)))
                {
                    document.Save(xw);
                }
            }
            if (addpagebreak)
                AddPageBreak(doc);
        }

        protected XDocument GetXDocument(WordprocessingDocument myDoc)
        {
            // Load the main document part into an XDocument
            XDocument mainDocumentXDoc;
            using (Stream str = myDoc.MainDocumentPart.GetStream())
            using (XmlReader xr = XmlReader.Create(str))
                mainDocumentXDoc = XDocument.Load(xr);
            return mainDocumentXDoc;
        }

        protected void AddPageBreak(WordprocessingDocument doc)
        {
            var pageBreak = new Paragraph(new Run(new Break() { Type = BreakValues.Page }));
            doc.MainDocumentPart.Document.Descendants<Paragraph>().Last().InsertAfterSelf(pageBreak);
        }


        protected ResultFileInfo MergeSources(List<Source> sources)
        {
            var merged = DocumentBuilder.BuildDocument(sources);
            var path = Path.GetTempFileName();
            merged.SaveAs(path);

            var bytes = File.ReadAllBytes(path);
            File.Delete(path);
            return new ResultFileInfo(this.FileName, this.MimeType, bytes);
        }
    }
}
