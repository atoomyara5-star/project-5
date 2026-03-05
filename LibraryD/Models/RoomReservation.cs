using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryD.Models
{
    public class RoomReservation
    {
        [Key]
        public int ReservationId { get; set; }

        public int UserId { get; set; }
        public int RoomId { get; set; }

        [Column("Reservation_Date")]
        public DateOnly ReservationDate { get; set; }

        [Column("Start_Time")]
        public TimeOnly StartTime { get; set; }

        [Column("End_Time")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public TimeOnly EndTime { get; set; }  // ✅ Computed

        public int StatusId { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public Room Room { get; set; }
        public Status Status { get; set; }
    }
}