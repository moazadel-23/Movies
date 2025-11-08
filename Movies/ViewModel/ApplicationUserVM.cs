using System.ComponentModel.DataAnnotations;

namespace Movies.ViewModel
{
    public class ApplicationUserVM
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string ImgPath { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
