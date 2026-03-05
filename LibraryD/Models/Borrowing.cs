using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models;

public partial class Borrowing
{
    [Key]
    public int BorrowId { get; set; }

    public int? UserId { get; set; }

    public int? BookId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? BorrowDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReturnDate { get; set; }

    public int? StatusId { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("Borrowings")]
    public virtual Book? Book { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("Borrowings")]
    public virtual Status? Status { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Borrowings")]
    public virtual User? User { get; set; }
}
