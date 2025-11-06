namespace Movies.Models
{
    public class ApplicationUserOTP
    {
        public string Id { get; set; }= string.Empty;
        public DateTime ValidTo { get; set; }
        public DateTime CreateAt { get; set; }
        public string OTP { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
