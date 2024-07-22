using System.ComponentModel.DataAnnotations;

namespace BlogManagementApp.Models
{
    public class BlogPostEdit
    {
        public short Bid { get; set; }

        public string SectionHedding { get; set; } = null!;

        public string? SectionImage { get; set; }

        public string? SectionDescription { get; set; }

        public DateOnly PostDate { get; set; }

        public short UploadUserId { get; set; }

        public short? CancelUserId { get; set; }

        public DateOnly? CancelDate { get; set; }

        public string? ReasonForCancel { get; set; }
        public string? EncId { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? BlogFile { get; set; }
    }
}
