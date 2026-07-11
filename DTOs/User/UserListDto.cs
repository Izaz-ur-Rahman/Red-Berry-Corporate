namespace RedBerryCorporate.DTOs.User
{
    public class UserListDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }

        public string ProfileImage { get; set; }
    }
}