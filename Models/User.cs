using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedBerryCorporate.Models
{
    [Table("User")] // VERY IMPORTANT (User is reserved keyword)
    public class User
    {
        [Key]
        public int ID { get; set; }

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(500)]
        public string? Password { get; set; }

        public int? EmpId { get; set; }

        [StringLength(100)]
        public string? Domain { get; set; }

        [StringLength(100)]
        public string? DomainUser { get; set; }

        [StringLength(100)]
        public string? UserType { get; set; }

        public int? Role { get; set; }

        public int? InspectRole { get; set; }

        [StringLength(100)]
        public string? AppId { get; set; }

        [StringLength(100)]
        public string? AppInspection { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int? Language { get; set; }

        [StringLength(50)]
        public string? AppWiseRoles { get; set; }

        [StringLength(50)]
        public string? AppIDs { get; set; }

        [StringLength(500)]
        public string? RoleNames { get; set; }

        [StringLength(50)]
        public string? DepartmentList { get; set; }

    }
}
