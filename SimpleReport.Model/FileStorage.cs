using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SimpleReport.Model
{
    public class FileStorage :IStorage
    {
        private string _filename;
        public FileStorage()
        {
            _filename = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["datastorefilename"];
        }
        
        public ReportManagerData LoadModel()
        {
            using (FileStream fs = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serializer.TypeNameHandling = TypeNameHandling.Objects;

                    TextReader treader = new StreamReader(fs);
                    JsonReader reader = new JsonTextReader(treader);
                    ReportManagerData data = serializer.Deserialize<ReportManagerData>(reader);
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not read Reports from file!", ex);
                }
            }
        }

        public void SaveModel(ReportManagerData data)
        {
            using (FileStream fs = File.Open(_filename, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, data);
            }
        }
    }
}
