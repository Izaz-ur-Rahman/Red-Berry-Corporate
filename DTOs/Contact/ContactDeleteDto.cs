using System.ComponentModel.DataAnnotations;

namespace RedBerryCorporate.DTOs.Contact
{
    public class ContactDeleteDto
    {
        [Required]
        public int Id { get; set; }
    }
}