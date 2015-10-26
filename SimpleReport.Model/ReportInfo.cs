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
        public Guid? AccessId { get; set; }
        [NonSerialized]
        public Access Access;
       

        public ReportInfo()
        {
            Id = Guid.NewGuid();
        }

    }

}