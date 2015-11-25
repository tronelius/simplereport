using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SimpleReport.Model.Storage
{
    public class StorageHelper : IStorageHelper
    {
        public void WriteModelToStream(ReportDataModel data, Stream stream)
        {
            using (StreamWriter sw = new StreamWriter(stream))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, data);
            }
        }

        public ReportDataModel ReadModelFromStream(Stream stream)
        {
            JsonSerializer serializer = new JsonSerializer();
            TextReader treader = new StreamReader(stream);
            JsonReader reader = new JsonTextReader(treader);
            ReportDataModel data = serializer.Deserialize<ReportDataModel>(reader);
            //todo handle empty file...
            return data;
        }
    }

    public interface IStorageHelper
    {
        void WriteModelToStream(ReportDataModel data, Stream stream);
        ReportDataModel ReadModelFromStream(Stream stream);
    }
}
