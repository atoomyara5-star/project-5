using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    public int FeedbackId { get; set; }

    public int? UserId { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string? Message { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Date { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Feedbacks")]
    public virtual User? User { get; set; }
}
