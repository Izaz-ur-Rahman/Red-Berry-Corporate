namespace RedBerryCorporate.DTOs.User
{
    public class ProfileDto
    {
        public int UserId { get; set; }

        public int EmployeeId { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string MobileNo { get; set; }

        public string WhatsappNo { get; set; }

        public string Position { get; set; }

        public string Bio { get; set; }

        public string Languages { get; set; }

        public string Facebook { get; set; }

        public string LinkedIn { get; set; }

        public string Twitter { get; set; }

        public string ProfileImage { get; set; }

        public bool IsActive { get; set; }
    }
}