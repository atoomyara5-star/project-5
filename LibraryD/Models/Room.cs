using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

public partial class Room
{
    [Key]
    public int RoomId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string RoomName { get; set; } = null!;

    public int Capacity { get; set; }

    public int? StatusId { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<RoomReservation> RoomReservations { get; set; } = new List<RoomReservation>();

    [ForeignKey("StatusId")]
    [InverseProperty("Rooms")]
    public virtual Status? Status { get; set; }
}
