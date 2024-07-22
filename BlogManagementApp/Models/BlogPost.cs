using System;
using System.Collections.Generic;

namespace BlogManagementApp.Models;

public partial class BlogPost
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

    public virtual UserList? CancelUser { get; set; }

    public virtual UserList UploadUser { get; set; } = null!;
}
