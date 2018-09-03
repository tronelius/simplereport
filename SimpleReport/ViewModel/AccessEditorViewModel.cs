using SimpleReport.Model;
using SimpleReport.Model.Constants;

namespace SimpleReport.ViewModel
{
    public class AccessEditorViewModel
    {
        public AccessStyle Value { get; set; }
        public string Text { get; set; }

        public AccessEditorViewModel(AccessStyle value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}