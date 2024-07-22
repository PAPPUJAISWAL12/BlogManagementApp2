using System.ComponentModel.DataAnnotations;

namespace BlogManagementApp.Models
{
    public class ChangePassword
    {
        [Required(ErrorMessage ="Please, Enter your New Password.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage ="Please,Enter your Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPasswod { get; set; } = null!;

        [Required(ErrorMessage ="Please,Enter your Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;

        public string? EmailToken { get; set; }

    }
}
