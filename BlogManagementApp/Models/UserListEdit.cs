using System.ComponentModel.DataAnnotations;

namespace BlogManagementApp.Models
{
    public class UserListEdit
    {
        public short UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string EmailAddress { get; set; } = null!;

        public string UserPhoto { get; set; } = null!;

        public string UserRole { get; set; } = null!;

        [DataType(DataType.Password)]
        public string UserPassword { get; set; } = null!;

        public string CurrentAddress { get; set; } = null!;

        public string EncId { get; set; } = string.Empty;

        [DataType(DataType.Upload)]
        public IFormFile? UserFile { get; set; }
        public string? EmailToken { get; set; }

    }
}
