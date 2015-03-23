namespace SimpleReport.Model.Storage
{
    public interface IStorage
    {
        ReportManagerData LoadModel();
        void SaveModel(ReportManagerData data);

    }
}
