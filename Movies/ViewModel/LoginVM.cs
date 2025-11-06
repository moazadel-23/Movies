using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Movies.ViewModel
{
    public class LoginVM
    {
        [Required]
        public string EmailOrUser { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
