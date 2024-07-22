using System;
using System.Collections.Generic;

namespace BlogManagementApp.Models;

public partial class UserList
{
    public short UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string UserPhoto { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string CurrentAddress { get; set; } = null!;

    public virtual ICollection<BlogPost> BlogPostCancelUsers { get; set; } = new List<BlogPost>();

    public virtual ICollection<BlogPost> BlogPostUploadUsers { get; set; } = new List<BlogPost>();
}
