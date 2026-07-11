namespace RedBerryCorporate.Models
{
    public class PasswordPolicy
    {
        public int ID { get; set; }
        public int? MinCharacter { get; set; }
        public int? MaxCharacter { get; set; }
        public int? MinNumCharacter { get; set; }
        public int? MaxSpecialCharacter { get; set; }
        public bool? IsPasswordExpired { get; set; }
        public string PasswordExpiredDuration { get; set; }
        public bool? LowerCase { get; set; }
        public bool? UpperCase { get; set; }
        public string SpecialCharAllowed { get; set; }
    }
}
