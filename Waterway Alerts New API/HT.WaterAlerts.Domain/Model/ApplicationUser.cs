using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HT.WaterAlerts.Domain
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string? MiddleName { get; set; }
        [StringLength(100)]
        public string LastName { get; set; }
        public bool SkipVideo { get; set; }
        public bool SkipTutorial { get; set; }
        public bool SkipTermsAndConditions { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }

    }
}
