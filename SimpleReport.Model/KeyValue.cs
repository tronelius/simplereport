using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model
{
    public class KeyValue
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public KeyValue()
        {
        }

        public KeyValue(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class IdName
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
