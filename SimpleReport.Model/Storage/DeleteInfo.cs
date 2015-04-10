using System.Collections.Generic;
using System.Linq;

namespace SimpleReport.Model.Storage
{
    public class DeleteInfo
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public string FullMessage
        {
            get
            {
                if (Entities != null)
                    return Message + " It's used in these entities:" + string.Join(",", Entities.ToList().Select(e => e.Name));
                else
                    return Message;
            }
        }

        public IEnumerable<IEntity> Entities {get; set; }

        public DeleteInfo()
        {
            Success = true;
        }

        public DeleteInfo(bool success, string message, IEnumerable<IEntity> entities) : this(success,message)
        {
            Entities = entities;
        }
        public DeleteInfo(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}