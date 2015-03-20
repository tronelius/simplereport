using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model.Extensions
{
    public static class EnumExtensions
    {
        public static Dictionary<int, string> ToDictionary(this Enum @enum)
        {
            var type = @enum.GetType();
            return Enum.GetValues(type).Cast<int>().ToDictionary(e => e, e => Enum.GetName(type, e));
        }

        public static IEnumerable<KeyValue> ToKeyValues(this Enum @enum)
        {
            List<KeyValue> returnlist = new List<KeyValue>();
            foreach (var element in @enum.ToDictionary())
            {
                yield return new KeyValue(element.Key,element.Value);
            }
        }
    }
}
