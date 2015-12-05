using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SimpleReport.Model.Storage.JsonConverters;

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
                var serializer = GetJsonSerializer();
                serializer.Serialize(jw, data);
            }
        }

        public ReportDataModel ReadModelFromStream(Stream stream)
        {
            var serializer = GetJsonSerializer();
            TextReader treader = new StreamReader(stream);
            JsonReader reader = new JsonTextReader(treader);
            ReportDataModel data = serializer.Deserialize<ReportDataModel>(reader);
            //todo handle empty file...
            return data;
        }
        private static JsonSerializer GetJsonSerializer()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new ParameterJsonConverter());
            return serializer;
        }


    }

    public interface IStorageHelper
    {
        void WriteModelToStream(ReportDataModel data, Stream stream);
        ReportDataModel ReadModelFromStream(Stream stream);
    }
}
