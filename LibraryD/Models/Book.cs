using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

public partial class Book
{
    [Key]
    public int BookId { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string BookName { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string? Author { get; set; }

    public int? CategoryId { get; set; }

    public int? StatusId { get; set; }

    [Column("picture")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Picture { get; set; }

    [InverseProperty("Book")]
    public virtual ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Books")]
    public virtual Category? Category { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("Books")]
    public virtual Status? Status { get; set; }
}
