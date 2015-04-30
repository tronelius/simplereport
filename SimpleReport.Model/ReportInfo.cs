using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleReport.Model
{
    public class ReportInfo : ValidatableEntity, IEntity
    {
        public Guid Id { get; set; }
        [Required][StringLength(300)] 
        public string Name { get; set; }
        [StringLength(50)]
        public string Group { get; set; }
        [StringLength(1000)] 
        public string Description { get; set; }
        
        ///AccessId=null=>Free for all!
        public Guid AccessId { get; set; }
        [NonSerialized]
        public Access Access;
        /*public List<ErrorInfo> Errors { get; private set; } 
        public bool CanExecute() { return Errors.Count(e => e.Level == ErrorLevel.Fatal)> 0;}
        public bool IsValid(){ return Errors.Count(e => e.Level > ErrorLevel.Warning) > 0;}*/

        public ReportInfo()
        {
            Id = Guid.NewGuid();
        }

        public ReportInfo(Guid id, string name, string description, string group)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception(string.Format("Missing name in report"));
            Id = id;
            Name = name;
            Description = description;
            Group = @group;
            //Errors = new List<ErrorInfo>();
        }

    }
}