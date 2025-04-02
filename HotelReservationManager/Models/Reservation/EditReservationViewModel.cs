using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Reservation
{
    public class EditReservationViewModel
    {
        public string Id { get; set; }
        public List<Data.Models.Room> AvaiableRooms { get; set; }
        public List<Data.Models.Client> AvaiableClients { get; set; }

        [Display(Name = "Номер на стая")]
        public string RoomId { get; set; }
        public string CreatorId { get; set; }

        [Display(Name = "Клиенти")]
        public List<string> ClientIds { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата на настаняване")]
        public DateTime CheckInTime { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата на освобождаване")]
        public DateTime CheckOutTime { get; set; }

        [Required]
        public bool Breakfast { get; set; }

        [Required]
        public bool AllInclusive { get; set; }
    }
}
