using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

public partial class Notification
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }
}
