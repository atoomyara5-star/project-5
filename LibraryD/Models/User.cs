using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

[Index("Email", Name = "UQ__Users__A9D1053488EDD400", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Role { get; set; }

    [Column("Profile_Picture")]
    [Unicode(false)]
    public string? ProfilePicture { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Blacklist> Blacklists { get; set; } = new List<Blacklist>();

    [InverseProperty("User")]
    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

    [InverseProperty("User")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("User")]
    public virtual ICollection<RoomReservation> RoomReservations { get; set; } = new List<RoomReservation>();
}
