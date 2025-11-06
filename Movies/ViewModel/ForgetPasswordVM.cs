using System.ComponentModel.DataAnnotations;

namespace Movies.ViewModel
{
    public class ForgetPasswordVM
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string UserNameOrEmail { get; set; } = string.Empty;
    }
}
