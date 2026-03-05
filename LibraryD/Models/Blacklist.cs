using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

[Table("Blacklist")]
public partial class Blacklist
{
    [Key]
    public int BlacklistId { get; set; }

    public int? UserId { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? Reason { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateAdded { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Blacklists")]
    public virtual User? User { get; set; }
}
