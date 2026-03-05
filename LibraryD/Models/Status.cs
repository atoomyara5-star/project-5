using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

[Table("Status")]
public partial class Status
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? StatusName { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    [InverseProperty("Status")]
    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

    [InverseProperty("Status")]
    public virtual ICollection<RoomReservation> RoomReservations { get; set; } = new List<RoomReservation>();

    [InverseProperty("Status")]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
