namespace RedBerryCorporate.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }

        public int EmpId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string MobileNo { get; set; }

        public string WhatsappNo { get; set; }

        public string Languages { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }

        public string ProfileImage { get; set; }
    }
}