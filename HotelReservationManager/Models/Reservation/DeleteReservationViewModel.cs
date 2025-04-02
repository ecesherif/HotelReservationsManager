using System.ComponentModel.DataAnnotations;

namespace HotelReservationManager.Models.Reservation
{
    public class DeleteReservationViewModel
        {
            public int Id { get; set; }

            [Display(Name = "Номер на стая")]
            public string RoomId { get; set; }

            [Display(Name = "Клиенти")]
            public List<string> ClientIds { get; set; }

            [Display(Name = "Дата на настаняване")]
            public DateTime CheckInTime { get; set; }

            [Display(Name = "Дата на освобождаване")]
            public DateTime CheckOutTime { get; set; }
        }
}
