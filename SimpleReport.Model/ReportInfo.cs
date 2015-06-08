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
        
        //who can fiddle with the template
        public Guid TemplateAccessId { get; set; }
        [NonSerialized]
        public Access TemplateAccess;

        public bool OnScreenFormatAllowed { get; set; }
        public TemplateEditor TemplateEditor { get; set; }

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

    public enum TemplateEditor
    {
        Anyone,
        ReportOwner,
        Administrators
    }
}