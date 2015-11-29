using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleReport.Model.Storage.JsonConverters
{
    //NOTE: We dont want to serialize the choices when saving to disk, but we do want to serialize them when serializing in the api. This seemed like an easy way to get that functionality, considering that we also might need to migrate things between datatypes in the future.
    public class ParameterJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            //we dont care about choices in this particular case.
            t["Choices"].Parent.Remove();

            t.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            //we dont care about choices in this particular case.
            var choices = jObject.Property("Choices");

            if(choices != null)
                choices.Remove();

            var target = new Parameter();
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Parameter) == objectType;
        }
    }
}