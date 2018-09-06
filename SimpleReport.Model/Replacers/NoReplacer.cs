namespace SimpleReport.Model.Replacers
{
    public class NoReplacer : IReplacer
    {
        public string Replace(string name)
        {
            return name;
        }
    }
}