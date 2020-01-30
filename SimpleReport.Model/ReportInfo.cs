using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

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

        //who can fiddle with the template
        public Guid ReportOwnerAccessId { get; set; }
        [NonSerialized]
        public Access ReportOwnerAccess; //TODO rename to reportowner

        public bool IsAvailableForMe(IPrincipal user, Access adminAccess)
        {
            return (ReportOwnerAccess != null && ReportOwnerAccess.IsAvailableForMe(user)) || (Access == null || Access.IsAvailableForMe(user) || adminAccess.IsAvailableForMe(user));
        }

        public ReportInfo()
        {
            Id = Guid.NewGuid();
        }

    }

}