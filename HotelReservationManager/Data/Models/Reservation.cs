using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Data.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required]
        public virtual Room Room { get; set; }
        public int RoomId { get; set; }

        [Required]
        public virtual User Creator { get; set; }
        public string CreatorId { get; set; }

        public virtual List<ClientReservation> ClientReservations { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата на настаняване")]
        public DateTime CheckInTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата на освобождаване")]
        public DateTime CheckOutTime { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CheckOutTime <= CheckInTime)
            {
                yield return new ValidationResult(
                    "Дата на освобождаване трябва да бъде след дата на настаняване!",
                    new[] { nameof(CheckOutTime) });
            }
        }

        [Required]
        public bool Breakfast { get; set; }


        [Required]
        [Display(Name = "All inclusive")]
        public bool AllInclusive { get; set; }


        [Required]
        [Display(Name = "Дължима сума")]
        public double TotalPrice { get; set; }
    }
}
